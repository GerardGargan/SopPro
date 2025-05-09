using System.Net;
using System.Text.Json;
using Backend.Data;
using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Models.Settings;
using Backend.Models.Tenancy;
using Backend.Repository.Interface;
using Backend.Service.Interface;
using Backend.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;

namespace Backend.Service.Implementation
{
    public class SopService : ISopService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenancyResolver _tenancyResolver;
        private readonly ApplicationDbContext _db;
        private readonly IBlobService _blobService;
        private readonly ApplicationSettings _appSettings;
        private readonly IEmailService _emailService;
        private readonly ITemplateService _templateService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IChatCompletionService _chatService;


        public SopService(IUnitOfWork unitOfWork, ITenancyResolver tenancyResolver, ApplicationDbContext db, IBlobService blobService, IOptions<ApplicationSettings> appSettings, IEmailService emailService, ITemplateService templateService, UserManager<ApplicationUser> userManager, IChatCompletionService chatService)
        {
            _unitOfWork = unitOfWork;
            _tenancyResolver = tenancyResolver;
            _db = db;
            _blobService = blobService;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _templateService = templateService;
            _userManager = userManager;
            _chatService = chatService;
        }

        /// <summary>
        /// Creates an SOP with a SOPVersion, associated Steps, Hazards and PPE
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<ApiResponse> CreateSop(SopDto model)
        {

            // Check if it already exists (by reference)
            var isSopExisting = await _unitOfWork.Sops.GetAsync(s => s.Reference == model.Reference.ToLower().Trim());
            if (isSopExisting != null)
            {
                throw new ArgumentException("Sop with this reference already exists");
            }

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                throw new ArgumentException("Title cant be empty");
            }

            if (string.IsNullOrWhiteSpace(model.Description))
            {
                throw new ArgumentException("Description cant be empty");
            }

            // Wrap database operations in a transaction
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Create sop record
                var sop = new Sop
                {
                    OrganisationId = _tenancyResolver.GetOrganisationid().Value,
                    DepartmentId = model.DepartmentId == 0 ? null : model.DepartmentId,
                    Reference = model.Reference.ToLower().Trim(),
                    isAiGenerated = model.isAiGenerated
                };

                await _unitOfWork.Sops.AddAsync(sop);
                await _unitOfWork.SaveAsync();

                // Create sop version record
                var sopVersion = new SopVersion
                {
                    SopId = sop.Id,
                    Version = 1,
                    AuthorId = _tenancyResolver.GetUserId(),
                    CreateDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    Title = model.Title,
                    Description = model.Description,
                    Status = SopStatus.Draft,
                    OrganisationId = _tenancyResolver.GetOrganisationid().Value,
                };
                await _unitOfWork.SopVersions.AddAsync(sopVersion);
                await _unitOfWork.SaveAsync();

                // Create SopHazard records

                if (model.SopHazards != null && model.SopHazards.Count > 0)
                {
                    var sopHazards = model.SopHazards.Select(sopHazard => new SopHazard
                    {
                        SopVersionId = sopVersion.Id,
                        Name = sopHazard.Name,
                        ControlMeasure = sopHazard.ControlMeasure,
                        RiskLevel = sopHazard.RiskLevel,
                        OrganisationId = _tenancyResolver.GetOrganisationid().Value
                    }).ToList();

                    await _unitOfWork.SopHazards.AddRangeAsync(sopHazards);
                }

                // Create SopStep records

                if (model.SopSteps != null && model.SopSteps.Count > 0)
                {
                    var sopSteps = CreateSteps(model, sopVersion.Id);
                    await _unitOfWork.SopSteps.AddRangeAsync(sopSteps);
                }

                await _unitOfWork.SaveAsync();
            });

            return new ApiResponse
            {
                StatusCode = HttpStatusCode.Created,
                IsSuccess = true,
                SuccessMessage = "Sop created successfully"
            };
        }

        /// <summary>
        /// Returns a list of all SOPs
        /// </summary>
        /// <param name="search"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="isFavourite"></param>
        /// <param name="soryBy"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        public async Task<ApiResponse<List<SopDto>>> GetAllSops(string search, int? status, int page, int pageSize, bool isFavourite = false, string soryBy = "recent", string sortOrder = "desc")
        {

            var query = _unitOfWork.Sops.GetAll();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(sop => sop.Reference.Contains(search) || sop.SopVersions.Any(sv => sv.Title.Contains(search) || sv.Description.Contains(search)));
            }

            if (status != null)
            {
                query = query.Where(sop => sop.SopVersions.OrderByDescending(x => x.Version).FirstOrDefault().Status == (SopStatus)status);
            }

            // Get a list of user favourited sop ids
            var userId = _tenancyResolver.GetUserId();
            var favouriteSopIds = await _unitOfWork.SopUserFavourites.GetAll(f => f.ApplicationUserId == userId).Select(f => f.SopId).ToListAsync();

            if (isFavourite)
            {
                query = query.Where(sop => favouriteSopIds.Contains(sop.Id));
            }

            // apply sorting
            if (soryBy.ToLower() == "recent")
            {
                query = sortOrder.ToLower() == "asc"
           ? query.OrderBy(sop => sop.SopVersions.OrderBy(x => x.LastUpdated).FirstOrDefault().LastUpdated)
           : query.OrderByDescending(sop => sop.SopVersions.OrderByDescending(x => x.LastUpdated).FirstOrDefault().LastUpdated);
            }

            var sops = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize).Select(sop => new SopDto
            {
                Id = sop.Id,
                Reference = sop.Reference,
                DepartmentId = sop.DepartmentId,
                isAiGenerated = sop.isAiGenerated,
                isFavourite = favouriteSopIds.Contains(sop.Id),
                SopVersions = sop.SopVersions.Select(sopVersion => new SopVersionDto
                {
                    Id = sopVersion.Id,
                    SopId = sop.Id,
                    Version = sopVersion.Version,
                    AuthorId = sopVersion.AuthorId,
                    CreateDate = sopVersion.CreateDate,
                    Title = sopVersion.Title,
                    Description = sopVersion.Description,
                    Status = sopVersion.Status,
                    SopHazards = sopVersion.SopHazards.Select(sopHazard => new SopHazardDto
                    {
                        Id = sopHazard.Id,
                        SopVersionId = sopHazard.SopVersionId,
                        Name = sopHazard.Name,
                        ControlMeasure = sopHazard.ControlMeasure,
                        RiskLevel = sopHazard.RiskLevel,
                    }).ToList(),
                    SopSteps = sopVersion.SopSteps.Select(sopStep => new SopStepDto
                    {
                        Id = sopStep.Id,
                        SopVersionId = sopStep.SopVersionId,
                        Position = sopStep.Position,
                        Text = sopStep.Text,
                        Title = sopStep.Title,
                        ImageUrl = sopStep.ImageUrl,
                    }).ToList()
                }).OrderByDescending(x => x.Version).ToList()
            }).ToListAsync();

            // add latest version to each sop
            foreach (var sop in sops)
            {
                var latestVersion = sop.SopVersions.OrderByDescending(sv => sv.Version).FirstOrDefault();
                if (latestVersion != null)
                {
                    sop.Title = latestVersion.Title;
                    sop.ImageUrl = latestVersion.SopSteps.FirstOrDefault()?.ImageUrl;
                    sop.Description = latestVersion.Description;
                    sop.IsApproved = latestVersion.Status == SopStatus.Approved;
                    sop.Version = latestVersion.Version;
                    sop.Status = latestVersion.Status;
                    sop.ImageUrl = latestVersion.SopSteps.OrderBy(s => s.Position).FirstOrDefault()?.ImageUrl;
                }
            }


            return new ApiResponse<List<SopDto>>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = sops
            };
        }

        /// <summary>
        /// Gets the latest SopVersion for a specified SOP
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse<SopDto>> GetLatestSopVersion(int id)
        {
            // Check that the SOP exists
            var sopEntity = await _unitOfWork.Sops.GetAsync(s => s.Id == id, includeProperties: "SopVersions,SopVersions.SopHazards,SopVersions.SopSteps,SopVersions.SopSteps.SopStepPpe");
            if (sopEntity == null)
            {
                throw new KeyNotFoundException("Sop not found");
            }

            var latestSopVersion = sopEntity.SopVersions
                .OrderByDescending(sv => sv.Version)
                .FirstOrDefault();

            if (latestSopVersion == null)
            {
                throw new KeyNotFoundException("No SopVersion found");
            }

            // Map to a DTO object
            var sopDto = new SopDto
            {
                Id = sopEntity.Id,
                Reference = sopEntity.Reference,
                Title = latestSopVersion.Title,
                Description = latestSopVersion.Description,
                IsApproved = latestSopVersion.Status == SopStatus.Approved,
                Status = latestSopVersion.Status,
                Version = latestSopVersion.Version,
                DepartmentId = sopEntity.DepartmentId ?? 0,
                isAiGenerated = sopEntity.isAiGenerated,
                SopSteps = latestSopVersion.SopSteps.Select(sopStep => new SopStepDto
                {
                    Id = sopStep.Id,
                    SopVersionId = sopStep.SopVersionId,
                    Position = sopStep.Position,
                    Text = sopStep.Text,
                    Title = sopStep.Title,
                    ImageUrl = sopStep.ImageUrl,
                    PpeIds = sopStep.SopStepPpe?.Select(x => x.PpeId).ToList()
                }).ToList(),
                SopHazards = latestSopVersion.SopHazards.Select(sopHazard => new SopHazardDto
                {
                    Id = sopHazard.Id,
                    SopVersionId = sopHazard.SopVersionId,
                    Name = sopHazard.Name,
                    ControlMeasure = sopHazard.ControlMeasure,
                    RiskLevel = sopHazard.RiskLevel,
                }).ToList(),
                SopVersions = new List<SopVersionDto>
                {
                    new SopVersionDto
                    {
                        Id = latestSopVersion.Id,
                        SopId = sopEntity.Id,
                        Version = latestSopVersion.Version,
                        AuthorId = latestSopVersion.AuthorId,
                        CreateDate = latestSopVersion.CreateDate,
                        Title = latestSopVersion.Title,
                        Description = latestSopVersion.Description,
                        Status = latestSopVersion.Status,
                        SopHazards = latestSopVersion.SopHazards.Select(sopHazard => new SopHazardDto
                        {
                            Id = sopHazard.Id,
                            SopVersionId = sopHazard.SopVersionId,
                            Name = sopHazard.Name,
                            ControlMeasure = sopHazard.ControlMeasure,
                            RiskLevel = sopHazard.RiskLevel,
                        }).ToList()
                    }
                }
            };

            return new ApiResponse<SopDto>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = sopDto
            };
        }

        /// <summary>
        /// Get a specific SopVersion by its id
        /// </summary>
        /// <param name="sopVersionId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<SopVersionDto> GetSopVersion(int sopVersionId)
        {
            var sopVersion = await _unitOfWork.SopVersions.GetAsync(x => x.Id == sopVersionId, includeProperties: "Author,ApprovedBy,SopSteps,SopSteps.SopStepPpe,SopHazards");

            if (sopVersion == null)
            {
                throw new KeyNotFoundException("Sop version not found");
            }

            SopVersionDto sopVersionDto = SopVersionDto.FromSopVersion(sopVersion);
            // emsure steps are in the correct order
            return sopVersionDto;
        }

        /// <summary>
        /// Updates a sop. Creates a new version if the existing version is approved, otherwise updates the existing version.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse<SopDto>> UpdateSop(int id, SopDto model)
        {
            ApiResponse<SopDto> response = new ApiResponse<SopDto>();

            // check if sop id exists
            if (id == 0 || model == null || model.Id == null)
            {
                throw new ArgumentException("Invalid id");
            }

            var sopFromDb = await _unitOfWork.Sops.GetAsync(s => s.Id == model.Id, tracked: true);
            if (sopFromDb == null)
            {
                throw new KeyNotFoundException("Sop not found");
            }

            // get latest sop version from db
            var latestVersion = await _unitOfWork.SopVersions.GetAll(sv => sv.SopId == model.Id, includeProperties: "SopHazards,SopSteps,SopSteps.SopStepPpe", tracked: true).OrderByDescending(sv => sv.Version).FirstOrDefaultAsync();

            if (latestVersion == null)
            {
                throw new KeyNotFoundException("No sop version found");
            }

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                sopFromDb.DepartmentId = model.DepartmentId == 0 ? null : model.DepartmentId;

                if (latestVersion.Status == SopStatus.Approved)
                {
                    // Create new version, duplicate steps, hazards
                    var newVersion = new SopVersion
                    {
                        SopId = latestVersion.SopId,
                        Version = latestVersion.Version + 1,
                        AuthorId = _tenancyResolver.GetUserId(),
                        OrganisationId = _tenancyResolver.GetOrganisationid().Value,
                        Title = model.Title,
                        Description = model.Description,
                        Status = SopStatus.Draft,
                        CreateDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };

                    await _unitOfWork.SopVersions.AddAsync(newVersion);
                    await _unitOfWork.SaveAsync();

                    // create sop steps with new version id
                    List<SopStep> sopSteps = CreateSteps(model, newVersion.Id);
                    await _unitOfWork.SopSteps.AddRangeAsync(sopSteps);

                    // create sop hazards from model
                    List<SopHazard> newHazards = CreateHazards(model, newVersion.Id);
                    await _unitOfWork.SopHazards.AddRangeAsync(newHazards);
                    await _unitOfWork.SaveAsync();

                    var newSop = await _unitOfWork.Sops.GetAsync(s => s.Id == model.Id, includeProperties: "SopVersions,SopVersions.SopHazards");

                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Result = SopDto.FromSop(newSop);

                }
                else
                {
                    // update existing version
                    latestVersion.Title = model.Title;
                    latestVersion.Description = model.Description;
                    latestVersion.LastUpdated = DateTime.UtcNow;

                    // update sop hazards
                    var existingHazardIds = latestVersion.SopHazards.Select(h => h.Id).ToList();
                    var modelHazardIds = model.SopHazards.Select(h => h.Id).ToList();

                    var hazardIdsToDelete = existingHazardIds.Where(id => !modelHazardIds.Contains(id)).ToList();
                    var hazardIdsToUpdate = existingHazardIds.Where(id => modelHazardIds.Contains(id)).ToList();
                    var hazardsToAdd = model.SopHazards.Where(hazard => hazard.Id == null || hazard.Id == 0).ToList();

                    // delete hazards
                    if (hazardIdsToDelete.Count > 0)
                    {
                        var hazardsToDelete = latestVersion.SopHazards.Where(h => hazardIdsToDelete.Contains(h.Id)).ToList();
                        _unitOfWork.SopHazards.RemoveRange(hazardsToDelete);
                    }

                    if (hazardIdsToUpdate.Count > 0)
                    {
                        // update hazards
                        foreach (var hazardId in hazardIdsToUpdate)
                        {
                            var hazard = latestVersion.SopHazards.FirstOrDefault(h => h.Id == hazardId);
                            var modelHazard = model.SopHazards.FirstOrDefault(h => h.Id == hazardId);

                            if (hazard != null && modelHazard != null)
                            {
                                hazard.Name = modelHazard.Name;
                                hazard.ControlMeasure = modelHazard.ControlMeasure;
                                hazard.RiskLevel = modelHazard.RiskLevel;
                            }
                        }
                    }

                    if (hazardsToAdd != null && hazardsToAdd.Count > 0)
                    {
                        // add new hazards
                        var newHazards = hazardsToAdd.Select(hazard => new SopHazard
                        {
                            SopVersionId = latestVersion.Id,
                            Name = hazard.Name,
                            ControlMeasure = hazard.ControlMeasure,
                            RiskLevel = hazard.RiskLevel,
                            OrganisationId = _tenancyResolver.GetOrganisationid().Value
                        }).ToList();

                        await _unitOfWork.SopHazards.AddRangeAsync(newHazards);
                    }

                    // update sop steps
                    var existingStepIds = latestVersion.SopSteps.Select(s => s.Id).ToList();
                    var modelStepIds = model.SopSteps.Select(s => s.Id).ToList();

                    var stepIdsToDelete = existingStepIds.Where(id => !modelStepIds.Contains(id)).ToList();
                    var stepIdsToUpdate = existingStepIds.Where(id => modelStepIds.Contains(id)).ToList();
                    var stepsToAdd = model.SopSteps.Where(step => step.Id == null || step.Id == 0).ToList();

                    // delete steps
                    if (stepIdsToDelete.Count > 0)
                    {
                        // delete any ppe records associated with the steps
                        var stepPpeToDelete = _db.SopStepPpe.Where(x => stepIdsToDelete.Contains(x.SopStepId)).ToList();
                        _db.SopStepPpe.RemoveRange(stepPpeToDelete);

                        // delete steps
                        var stepsToDelete = latestVersion.SopSteps.Where(s => stepIdsToDelete.Contains(s.Id)).ToList();
                        _unitOfWork.SopSteps.RemoveRange(stepsToDelete);
                    }

                    // update steps
                    if (stepIdsToUpdate.Count > 0)
                    {
                        foreach (var stepId in stepIdsToUpdate)
                        {
                            var step = latestVersion.SopSteps.FirstOrDefault(s => s.Id == stepId);
                            var modelStep = model.SopSteps.FirstOrDefault(s => s.Id == stepId);

                            if (step != null && modelStep != null)
                            {
                                step.Position = modelStep.Position;
                                step.ImageUrl = modelStep.ImageUrl;
                                step.Text = modelStep.Text;
                                step.Title = modelStep.Title;
                            }
                        }
                    }

                    // update step ppe
                    foreach (var step in latestVersion.SopSteps)
                    {
                        var modelStep = model.SopSteps.FirstOrDefault(s => s.Id == step.Id);
                        if (modelStep != null)
                        {
                            var existingPpeIds = step.SopStepPpe.Select(x => x.PpeId).ToList();
                            var modelPpeIds = modelStep.PpeIds ?? new List<int>();

                            var ppeIdsToDelete = existingPpeIds.Where(id => !modelPpeIds.Contains(id)).ToList();
                            var ppeIdsToAdd = modelPpeIds.Where(id => !existingPpeIds.Contains(id)).ToList();

                            // delete ppe
                            if (ppeIdsToDelete.Count > 0)
                            {
                                var ppeToDelete = step.SopStepPpe.Where(x => ppeIdsToDelete.Contains(x.PpeId)).ToList();
                                _db.SopStepPpe.RemoveRange(ppeToDelete);
                            }

                            // add ppe
                            if (ppeIdsToAdd.Count > 0)
                            {
                                var ppeToAdd = ppeIdsToAdd.Select(ppeId => new SopStepPpe
                                {
                                    SopStepId = step.Id,
                                    PpeId = ppeId,
                                    OrganisationId = _tenancyResolver.GetOrganisationid().Value
                                }).ToList();

                                _db.SopStepPpe.AddRange(ppeToAdd);
                            }
                        }
                    }

                    // create new steps
                    if (stepsToAdd != null && stepsToAdd.Count > 0)
                    {
                        var newSteps = CreateSteps(stepsToAdd, latestVersion.Id);
                        await _unitOfWork.SopSteps.AddRangeAsync(newSteps);
                    }

                    await _unitOfWork.SaveAsync();

                    var updatedSop = await _unitOfWork.Sops.GetAsync(s => s.Id == model.Id, includeProperties: "SopVersions,SopVersions.SopHazards");

                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Result = SopDto.FromSop(updatedSop);

                }
            });

            return response;

        }

        /// <summary>
        /// Generates an SOP using an AI Prompt
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<SopDto> GenerateAiSop(AiRequestDto model)
        {
            // Create JSON Schema with desired response type from string.
            ChatResponseFormat chatResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
            "sop_structure",
            jsonSchema: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "SopVersion": {
                        "type": "object",
                        "properties": {
                            "Title": { "type": "string" },
                            "Description": { "type": "string" },
                            "SopSteps": {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "properties": {
                                        "Position": { "type": ["integer", "null"] },
                                        "Title": { "type": "string" },
                                        "Text": { "type": "string" }
                                    },
                                    "required": ["Position", "Title", "Text"],
                                    "additionalProperties": false
                                }
                            },
                            "SopHazards": {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "properties": {
                                        "Name": { "type": "string" },
                                        "ControlMeasure": { "type": "string" },
                                        "RiskLevel": {
                                            "type": ["integer", "null"],
                                            "enum": [0, 1, 2]
                                        }
                                    },
                                    "required": ["Name", "ControlMeasure", "RiskLevel"],
                                    "additionalProperties": false
                                }
                            }
                        },
                        "required": ["Title", "Description", "SopSteps", "SopHazards"],
                        "additionalProperties": false
                    }
                },
                "required": ["SopVersion"],
                "additionalProperties": false
            }
            """), null, true);

#pragma warning disable SKEXP0010 
            var executionSettings = new OpenAIPromptExecutionSettings
            {
                ResponseFormat = chatResponseFormat
            };
#pragma warning restore SKEXP0010 

            var fullPrompt =
  "You are an assistant that generates SOPs (Standard Operating Procedures) as JSON data. The response must conform to the provided schema. " +
  "Each SOP should contain SopVersion (with Title and Description), SopSteps (array with Position, Title, and Text), and SopHazards " +
  "(array with Name, ControlMeasure, and RiskLevel [Low, Medium, High]). Use the following mapping for 'RiskLevel' in each hazard: 1 = Low, 2 = Medium, 3 = High - always return the integer, not the text. " +
  "When generating the SOP, take into consideration the provided task, primary goal, key considerations, and potential risks. These inputs are meant to guide you in writing a clear and complete SOP. " +
  $"Return a valid JSON object. Here is the information provided about the sop from the user: \n\n Description of the job: {model.JobDescription} \n\n PrimaryGoal: {model.PrimaryGoal} \n\n Key risks or considerations: {model.KeyRisks}";

            var result = await _chatService.GetChatMessageContentAsync(fullPrompt, executionSettings);

            string jsonResult = result.ToString();

            AiSop sopAiDto = System.Text.Json.JsonSerializer.Deserialize<AiSop>(jsonResult, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            });

            SopDto sopDto = new SopDto();

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                Sop sop = new Sop()
                {
                    Reference = new Random().Next(10000000).ToString(),
                    DepartmentId = null,
                    isAiGenerated = true,
                    OrganisationId = _tenancyResolver.GetOrganisationid().Value,
                };

                await _unitOfWork.Sops.AddAsync(sop);
                await _unitOfWork.SaveAsync();

                SopVersion sopVersion = new SopVersion()
                {
                    SopId = sop.Id,
                    Version = 1,
                    Status = SopStatus.Draft,
                    AuthorId = _tenancyResolver.GetUserId(),
                    CreateDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    Title = sopAiDto.SopVersion.Title,
                    Description = sopAiDto.SopVersion.Description,
                    OrganisationId = _tenancyResolver.GetOrganisationid().Value
                };

                await _unitOfWork.SopVersions.AddAsync(sopVersion);
                await _unitOfWork.SaveAsync();

                List<SopStep> sopSteps = new List<SopStep>(sopAiDto.SopVersion.SopSteps.Count);

                int position = 1;
                foreach (var step in sopAiDto.SopVersion.SopSteps)
                {
                    var sopStep = new SopStep()
                    {
                        SopVersionId = sopVersion.Id,
                        Position = position,
                        Title = step.Title,
                        Text = step.Text,
                        OrganisationId = _tenancyResolver.GetOrganisationid().Value
                    };
                    sopSteps.Add(sopStep);
                    position += 1;
                }
                await _unitOfWork.SopSteps.AddRangeAsync(sopSteps);

                List<SopHazard> sopHazards = new List<SopHazard>(sopAiDto.SopVersion.SopHazards.Count);

                foreach (var hazard in sopAiDto.SopVersion.SopHazards)
                {
                    var sopHazard = new SopHazard()
                    {
                        SopVersionId = sopVersion.Id,
                        Name = hazard.Name,
                        ControlMeasure = hazard.ControlMeasure,
                        RiskLevel = (RiskLevel)hazard.RiskLevel,
                        OrganisationId = _tenancyResolver.GetOrganisationid().Value
                    };

                    sopHazards.Add(sopHazard);
                }
                await _unitOfWork.SopHazards.AddRangeAsync(sopHazards);


                await _unitOfWork.SaveAsync();

                sopDto = SopDto.FromSop(sop);
            });

            return sopDto;
        }

        /// <summary>
        /// Deletes sops and associated data from the database and blob storage
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse> DeleteSops(List<int> ids)
        {
            var response = new ApiResponse();

            var sops = await _unitOfWork.Sops.GetAll(x => ids.Contains(x.Id)).ToListAsync();
            var validSops = sops.Where(x => x != null).ToList();
            var sopIds = sops.Select(x => x.Id).ToList();

            if (sops == null || sops.Count == 0)
            {
                throw new KeyNotFoundException("No sops found");
            }

            // Get all sopVersions and Ids
            var sopVersions = await _unitOfWork.SopVersions.GetAll(x => sopIds.Contains(x.SopId)).ToListAsync();
            var sopVersionIds = sopVersions.Select(x => x.Id).ToList();

            // Get all associated sopHazards, sopSteps and PPE
            var sopHazards = await _unitOfWork.SopHazards.GetAll(x => sopVersionIds.Contains(x.SopVersionId)).ToListAsync();
            var sopSteps = await _unitOfWork.SopSteps.GetAll(x => sopVersionIds.Contains(x.SopVersionId)).ToListAsync();
            var sopStepIds = sopSteps.Select(x => x.Id).ToList();

            var sopStepPpe = _db.SopStepPpe.Where(x => sopStepIds.Contains(x.SopStepId)).ToList();

            // Get a list of images to delete from BLOB storage
            var imageUrisToDelete = sopSteps.Where(x => !string.IsNullOrWhiteSpace(x.ImageUrl)).Select(x => x.ImageUrl).ToList();
            var UserSopFavourites = await _unitOfWork.SopUserFavourites.GetAll(x => sopIds.Contains(x.SopId)).ToListAsync();

            // perform deletion in order to avoid FK constraint errors
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // delete user sop favourite records
                if (UserSopFavourites.Count > 0)
                {
                    _unitOfWork.SopUserFavourites.RemoveRange(UserSopFavourites);
                }

                // delete sop step ppe
                if (sopStepPpe.Count > 0)
                {
                    _db.SopStepPpe.RemoveRange(sopStepPpe);
                }

                // delete sop steps
                if (sopStepIds.Count > 0)
                {
                    _unitOfWork.SopSteps.RemoveRange(sopSteps);
                }

                if (sopHazards.Count > 0)
                {
                    // delete sop hazards
                    _unitOfWork.SopHazards.RemoveRange(sopHazards);
                }

                if (sopVersionIds.Count > 0)
                {
                    // delete sop versions
                    _unitOfWork.SopVersions.RemoveRange(sopVersions);
                }

                // delete sop
                _unitOfWork.Sops.RemoveRange(validSops);

                await _unitOfWork.SaveAsync();

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.SuccessMessage = $"Sops deleted successfully";
            });

            // delete images from blob storage

            foreach (var blobUrl in imageUrisToDelete)
            {
                Uri uri = new Uri(blobUrl);
                string blobName = string.Join("", uri.Segments.Skip(2));
                var deleted = await _blobService.DeleteBlob(blobName, _appSettings.AzureBlobStorageContainer);
            }
            ;

            return response;
        }


        /// <summary>
        /// Reverts an SOP to a previous version
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task RevertSop(RevertRequestDto model)
        {
            // fetch the sopVersion
            SopVersion sopVersionFromDb = await _unitOfWork.SopVersions.GetAsync(x => x.Id == model.versionId);

            if (sopVersionFromDb == null)
            {
                throw new KeyNotFoundException("Version not found");
            }

            int sopId = sopVersionFromDb.SopId;

            // fetch all sopVersions for this sop
            List<SopVersion> allVersions = await _unitOfWork.SopVersions.GetAll(x => x.SopId == sopId).OrderByDescending(x => x.Version).ToListAsync();

            if (!allVersions.Any())
            {
                throw new KeyNotFoundException("No versions found for the specified SOP");
            }

            // check if the version chosen is the current version
            if (allVersions.FirstOrDefault().Id == sopVersionFromDb.Id)
            {
                throw new ArgumentException("Error - Version selected is the current version");
            }

            // Delete versions, and associated ppe, steps, hazards in the correct order for FK relationships
            List<SopVersion> versionsToDelete = allVersions.Where(x => x.Version > sopVersionFromDb.Version).ToList();
            var sopVersionIds = versionsToDelete.Select(x => x.Id).ToList();

            var sopHazards = await _unitOfWork.SopHazards.GetAll(x => sopVersionIds.Contains(x.SopVersionId)).ToListAsync();
            var sopSteps = await _unitOfWork.SopSteps.GetAll(x => sopVersionIds.Contains(x.SopVersionId)).ToListAsync();
            var sopStepIds = sopSteps.Select(x => x.Id).ToList();
            var sopStepPpe = _db.SopStepPpe.Where(x => sopStepIds.Contains(x.SopStepId)).ToList();

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // delete sop step ppe
                if (sopStepPpe.Count > 0)
                {
                    _db.SopStepPpe.RemoveRange(sopStepPpe);
                }

                // delete sop steps
                if (sopStepIds.Count > 0)
                {
                    _unitOfWork.SopSteps.RemoveRange(sopSteps);
                }

                if (sopHazards.Count > 0)
                {
                    // delete sop hazards
                    _unitOfWork.SopHazards.RemoveRange(sopHazards);
                }

                if (sopVersionIds.Count > 0)
                {
                    // delete sop versions
                    _unitOfWork.SopVersions.RemoveRange(versionsToDelete);
                }

                await _unitOfWork.SaveAsync();
            });
        }

        /// <summary>
        /// Adds a sop to the user's favourites
        /// </summary>
        /// <param name="id">The ID of the SOP to be added</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse> AddToFavourites(int id)
        {
            // Check that the sop exists
            var sop = await _unitOfWork.Sops.GetAsync(s => s.Id == id);
            if (sop == null)
            {
                throw new KeyNotFoundException("Sop not found");
            }

            // Get the user id
            var userId = _tenancyResolver.GetUserId();

            var SopUserFavourite = new SopUserFavourite
            {
                SopId = id,
                ApplicationUserId = userId,
                OrganisationId = _tenancyResolver.GetOrganisationid().Value
            };

            // Check if it is a duplicate
            var duplicateLookup = await _unitOfWork.SopUserFavourites.GetAsync(f => f.SopId == id && f.ApplicationUserId == userId);
            if (duplicateLookup != null)
            {
                throw new ArgumentException("Sop is already favourited");
            }

            // Add the sop to favourites
            await _unitOfWork.SopUserFavourites.AddAsync(SopUserFavourite);
            await _unitOfWork.SaveAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                SuccessMessage = "Sop added to favourites"
            };
        }

        /// <summary>
        /// Removes a sop from the user's favourites
        /// </summary>
        /// <param name="id">The ID of the SOP to be removed</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse> RemoveFromFavourites(int id)
        {
            // Get the user id
            var userId = _tenancyResolver.GetUserId();

            // Check that the sop favourite record exists
            var sopFavourite = await _unitOfWork.SopUserFavourites.GetAsync(x => x.SopId == id && x.ApplicationUserId == userId);

            if (sopFavourite == null)
            {
                throw new KeyNotFoundException("Sop favourite not found");
            }

            // delete the record
            _unitOfWork.SopUserFavourites.Remove(sopFavourite);
            await _unitOfWork.SaveAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                SuccessMessage = "Sop removed from favourites"
            };
        }

        /// <summary>
        /// Remove all Favourited SOPs for a specific user by their id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isSave"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task RemoveAllUserFavourites(string userId, bool isSave)
        {
            if (userId == null)
            {
                throw new ArgumentException("User id cant be null");
            }

            var favouritesFromDb = await _unitOfWork.SopUserFavourites.GetAll(x => x.ApplicationUserId == userId).ToListAsync();
            if (favouritesFromDb.Count > 0)
            {
                _unitOfWork.SopUserFavourites.RemoveRange(favouritesFromDb);

                if (isSave)
                {
                    await _unitOfWork.SaveAsync();
                }
            }
        }

        /// <summary>
        /// Updates an SOPs status to Approved and notifies the author
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiResponse> ApproveSop(int id)
        {
            // Update the SOP to Approved
            var updatedSop = await UpdateLatestVersionStatus(id, SopStatus.Approved);

            var latestVersion = updatedSop.SopVersions
                .OrderByDescending(sv => sv.Version)
                .FirstOrDefault();

            // Get relevant data needed for email notification
            var authorId = latestVersion.AuthorId;
            var approverId = latestVersion.ApprovedById;

            var approver = await _unitOfWork.ApplicationUsers.GetAsync(x => x.Id == approverId);

            if (!string.IsNullOrWhiteSpace(authorId))
            {
                var author = await _unitOfWork.ApplicationUsers.GetAsync(x => x.Id == authorId);
                if (!string.IsNullOrWhiteSpace(author.Email))
                {
                    var model = new
                    {
                        AuthorForename = author.Forename,
                        ApproverForename = approver.Forename,
                        ApproverSurname = approver.Surname,
                        Title = latestVersion.Title,
                        ApprovalDate = latestVersion.ApprovalDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A",
                        Reference = updatedSop.Reference
                    };

                    // Render the email template html
                    string emailBody = await _templateService.RenderTemplateAsync("SopApproved", model);

                    // Send email
                    _ = _emailService.SendEmailAsync(author.Email, "Sop approved", emailBody);
                }
            }


            return new ApiResponse()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                SuccessMessage = "Sop approved"
            };
        }

        /// <summary>
        /// Updates an SOPs status to Rejected and notifies the author
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiResponse> RejectSop(int id)
        {
            // Update status to rejected
            var updatedSop = await UpdateLatestVersionStatus(id, SopStatus.Rejected);
            var latestVersion = updatedSop.SopVersions
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();

            // Construct email
            string authorId = latestVersion.AuthorId;
            var rejectedByUserId = _tenancyResolver.GetUserId();

            var rejectedByUser = await _unitOfWork.ApplicationUsers.GetAsync(x => x.Id == rejectedByUserId);

            if (!string.IsNullOrWhiteSpace(authorId))
            {
                var author = await _unitOfWork.ApplicationUsers.GetAsync(x => x.Id == authorId);
                if (!string.IsNullOrWhiteSpace(author.Email))
                {
                    var model = new
                    {
                        AuthorForename = author.Forename,
                        RejectorForename = rejectedByUser.Forename,
                        RejectorSurname = rejectedByUser.Surname,
                        Title = latestVersion.Title,
                        Date = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") ?? "N/A",
                        Reference = updatedSop.Reference
                    };

                    string emailBody = await _templateService.RenderTemplateAsync("SopRejected", model);
                    // Send email
                    _ = _emailService.SendEmailAsync(author.Email, "Sop rejected", emailBody);
                }
            }

            return new ApiResponse()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                SuccessMessage = "Sop rejected"
            };
        }

        /// <summary>
        /// Requests approval for an SOP. Updates its status to In Review and emails administrators to request approval.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiResponse> RequestApproval(int id)
        {
            var updatedSop = await UpdateLatestVersionStatus(id, SopStatus.InReview);
            var latestVersion = updatedSop.SopVersions
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();

            // Send email notification to administrators
            var adminUsers = await _userManager.GetUsersInRoleAsync(StaticDetails.Role_Admin);
            var adminEmails = adminUsers.Where(x => !string.IsNullOrWhiteSpace(x.Email)).Select(x => x.Email).ToList();

            var authorId = latestVersion.AuthorId;
            var requestorUserId = _tenancyResolver.GetUserId();

            if (adminUsers != null && adminUsers.Count > 0)
            {
                var author = await _unitOfWork.ApplicationUsers.GetAsync(x => x.Id == authorId);
                var requestor = await _unitOfWork.ApplicationUsers.GetAsync(x => x.Id == requestorUserId);

                var model = new
                {
                    Requestor = $"{requestor.Forename} {requestor.Surname}",
                    Author = $"{author.Forename} {author.Surname}",
                    Title = latestVersion.Title,
                    Date = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") ?? "N/A",
                    Reference = updatedSop.Reference
                };

                string emailBody = await _templateService.RenderTemplateAsync("SopApprovalRequest", model);

                _ = _emailService.SendEmailAsync(adminEmails, null, "Sop approval request", emailBody);

            }

            return new ApiResponse()
            {
                IsSuccess = true,
                SuccessMessage = "Sop sent for review",
                StatusCode = HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Helper method which takes an SOP's Id, fetches the latest version and updates its status to the specified status provided validation checks are passed.
        /// </summary>
        /// <param name="sopId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Sop> UpdateLatestVersionStatus(int sopId, SopStatus status)
        {
            var sopEntity = await _unitOfWork.Sops.GetAsync(s => s.Id == sopId, includeProperties: "SopVersions", tracked: true);
            if (sopEntity == null)
            {
                throw new KeyNotFoundException("Sop not found");
            }

            var latestSopVersion = sopEntity.SopVersions
                .OrderByDescending(sv => sv.Version)
                .FirstOrDefault();

            if (latestSopVersion == null)
            {
                throw new KeyNotFoundException("No SopVersion found");
            }

            // Only allow InReview SOPs to be approved
            if (status == SopStatus.Approved && latestSopVersion.Status != SopStatus.InReview)
            {
                throw new ArgumentException("Invalid status for approval");
            }

            // Only allow InReview SOPs to be rejected
            if (status == SopStatus.Rejected && latestSopVersion.Status != SopStatus.InReview)
            {
                throw new ArgumentException("Invalid status for rejection");
            }

            // Only allow draft or rejected SOPs to request approval
            if (status == SopStatus.InReview && latestSopVersion.Status != SopStatus.Draft && latestSopVersion.Status != SopStatus.Rejected)
            {
                throw new ArgumentException("Invalid status for requesting approval");
            }

            latestSopVersion.Status = status;

            if (status == SopStatus.Approved)
            {
                latestSopVersion.ApprovalDate = DateTime.UtcNow;
                latestSopVersion.ApprovedById = _tenancyResolver.GetUserId();
            }

            await _unitOfWork.SaveAsync();

            return sopEntity;
        }

        /// <summary>
        /// Gets aggregated statistics for SOP analytics
        /// </summary>
        /// <returns></returns>
        public async Task<AnalyticsResponseDto> GetAnalytics()
        {
            List<Sop> sops = await _unitOfWork.Sops.GetAll(includeProperties: "SopVersions,Department").ToListAsync();

            int totalSops = sops.Count;

            // Get a list of the most recent sop version for each sop
            List<SopVersion> latestSopVersions = sops
                .Where(sop => sop.SopVersions.Any())
                .Select(sop => sop.SopVersions.OrderByDescending(v => v.Version).FirstOrDefault())
                .ToList();

            IEnumerable<SopVersion> flattenedVersions = sops
            .SelectMany(sop => sop.SopVersions);

            int totalApproved = flattenedVersions.Count(sv => sv.Status == SopStatus.Approved);
            int totalInReview = flattenedVersions.Count(sv => sv.Status == SopStatus.InReview);
            double approvalRate = (totalSops > 0) ? ((double)totalApproved / totalSops) * 100 : 0;

            int totalDraft = flattenedVersions.Count(sv => sv.Status == SopStatus.Draft);
            int totalRejected = flattenedVersions.Count(sv => sv.Status == SopStatus.Rejected);

            // Generate the last 12 months, starting from the current month
            var last12Months = Enumerable.Range(0, 12)
                .Select(i => DateTime.UtcNow.AddMonths(-i)) // Create the last 12 months
                .OrderBy(date => date)
                .Select(date => new
                {
                    MonthYearLabel = $"{date:MM/yyyy}", // Format as MM/YYYY
                    Month = date.Month,
                    Year = date.Year
                })
                .ToList();

            // Group the data by year and month
            var monthlyData = flattenedVersions
                .Where(sv => sv.CreateDate >= DateTime.UtcNow.AddMonths(-12)) // Filter versions within the last 12 months
                .GroupBy(sv => new { sv.CreateDate.Value.Year, sv.CreateDate.Value.Month }) // Group by year and month
                .Select(g => new
                {
                    MonthYearLabel = $"{g.Key.Month:D2}/{g.Key.Year}", // Format as MM/YYYY
                    Count = g.Count()
                })
                .ToList();

            // Merge the last 12 months with the actual data, filling in zeros for months with no data
            var mergedData = last12Months.Select(month => new
            {
                MonthYearLabel = month.MonthYearLabel, // MM/YYYY format
                Count = monthlyData.FirstOrDefault(d => d.MonthYearLabel == month.MonthYearLabel)?.Count ?? 0
            }).ToList();

            Dictionary<int, string> departmentDict = await _unitOfWork.Departments.GetAll().ToDictionaryAsync(x => x.Id, x => x.Name);

            var departmentData = sops
                .GroupBy(x => x.DepartmentId)
                .Select(x => new
                {
                    DepartmentId = x.Key,
                    DepartmentName = x.Key != null ? departmentDict[x.Key.Value] : "None",
                    Count = x.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            var barData = new ChartData
            {
                Labels = departmentData.Select(x => x.DepartmentName).ToList(),
                Datasets = new List<ChartDataset>
                {
                    new ChartDataset {
                        Data = departmentData.Select(x => x.Count).ToList()
                    }
                }
            };

            var lineData = new ChartData
            {
                Labels = mergedData.Select(x => x.MonthYearLabel).ToList(),
                Datasets = new List<ChartDataset>
                {
                    new ChartDataset
                    {
                        Data = mergedData.Select(x => x.Count).ToList(),
                    }
                }
            };



            List<SummaryCardData> summaryCards = new List<SummaryCardData>(3)
            {
                new SummaryCardData()
                {
                    Title = "Total SOPs",
                    Value = totalSops.ToString(),
                    Subtitle = "All categories"
                },
                new SummaryCardData()
                {
                    Title = "Approval Rate",
                    Value = $"{approvalRate:0}%",
                    Subtitle = "SOPs approved"
                },
                new SummaryCardData()
                {
                    Title = "Under Review",
                    Value = totalInReview.ToString(),
                    Subtitle = "Pending approval"
                },
                new SummaryCardData()
                {
                    Title = "Drafts",
                    Value = totalDraft.ToString(),
                    Subtitle = "SOPs drafted"
                }
            };

            List<PieChartData> pieChartData = new List<PieChartData>()
            {
                new PieChartData()
                {
                    Name = "Approved",
                    Population = totalApproved,
                    Color = "#00C49F",
                },
                new PieChartData()
                {
                    Name = "Draft",
                    Population = totalDraft,
                    Color = "#0088FE",
                },
                new PieChartData()
                {
                    Name = "In Review",
                    Population = totalInReview,
                    Color = "#FFBB28",
                },
                new PieChartData()
                {
                    Name = "Rejected",
                    Population = totalRejected,
                    Color = "#FF4C4C",
                },
            };

            AnalyticsResponseDto analyticsDto = new AnalyticsResponseDto()
            {
                SummaryCards = summaryCards,
                PieData = pieChartData,
                LineData = lineData,
                BarData = barData
            };

            return analyticsDto;
        }


        /// <summary>
        /// Uploads an image file to BLOB storage
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse> UploadImage(FileDto file)
        {
            if (file.File == null)
            {
                throw new ArgumentException("File is required");
            }
            var fileName = $"{Guid.NewGuid()}_{file.File.FileName}";

            var path = await _blobService.UploadBlob(fileName, _appSettings.AzureBlobStorageContainer, file.File);

            return new ApiResponse<string>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = path
            };
        }

        /// <summary>
        /// Helper method to create a list of hazard entities
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sopVersionId"></param>
        /// <returns></returns>
        private List<SopHazard> CreateHazards(SopDto model, int sopVersionId)
        {
            return model.SopHazards.Select(hazard => new SopHazard
            {
                SopVersionId = sopVersionId,
                Name = hazard.Name,
                ControlMeasure = hazard.ControlMeasure,
                RiskLevel = hazard.RiskLevel,
                OrganisationId = _tenancyResolver.GetOrganisationid().Value
            }).ToList();
        }

        /// <summary>
        /// Helper method to create a list of SopSteps from a SopDto object
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sopVersionId"></param>
        /// <returns></returns>
        public List<SopStep> CreateSteps(SopDto model, int sopVersionId)
        {
            return model.SopSteps.Select(step => new SopStep
            {
                SopVersionId = sopVersionId,
                Position = step.Position,
                ImageUrl = step.ImageUrl,
                Text = step.Text,
                Title = step.Title,
                OrganisationId = _tenancyResolver.GetOrganisationid().Value,
                SopStepPpe = step.PpeIds?.Select(ppeId => new SopStepPpe
                {
                    PpeId = ppeId,
                    OrganisationId = _tenancyResolver.GetOrganisationid().Value
                }).ToList()
            }).ToList();
        }

        public List<SopStep> CreateSteps(List<SopStepDto> model, int sopVersionId)
        {
            return model.Select(step => new SopStep
            {
                SopVersionId = sopVersionId,
                Position = step.Position,
                ImageUrl = step.ImageUrl,
                Text = step.Text,
                Title = step.Title,
                OrganisationId = _tenancyResolver.GetOrganisationid().Value
            }).ToList();
        }

        /// <summary>
        /// Creates duplicate steps from a provided SopVersion model
        /// </summary>
        /// <param name="sopVersion"></param>
        /// <param name="sopVersionId"></param>
        /// <returns></returns>
        private List<SopStep> DuplicateSteps(SopVersion sopVersion, int sopVersionId)
        {
            return sopVersion.SopSteps.Select(s => new SopStep
            {
                SopVersionId = sopVersionId,
                Position = s.Position,
                ImageUrl = s.ImageUrl,
                Text = s.Text,
                Title = s.Title,
                OrganisationId = _tenancyResolver.GetOrganisationid().Value,
            }).ToList();
        }
    }
}