using System.Net;
using Backend.Data;
using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Models.Settings;
using Backend.Models.Tenancy;
using Backend.Repository.Interface;
using Backend.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backend.Service.Implementation
{
    public class SopService : ISopService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenancyResolver _tenancyResolver;
        private readonly ApplicationDbContext _db;
        private readonly IBlobService _blobService;
        private readonly ApplicationSettings _appSettings;

        public SopService(IUnitOfWork unitOfWork, ITenancyResolver tenancyResolver, ApplicationDbContext db, IBlobService blobService, IOptions<ApplicationSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _tenancyResolver = tenancyResolver;
            _db = db;
            _blobService = blobService;
            _appSettings = appSettings.Value;
        }

        public async Task<ApiResponse> CreateSop(SopDto model)
        {

            var isSopExisting = await _unitOfWork.Sops.GetAsync(s => s.Reference == model.Reference.ToLower().Trim());
            if (isSopExisting != null)
            {
                throw new Exception("Sop with this reference already exists");
            }

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
                IsSuccess = true,
                SuccessMessage = "Sop created successfully"
            };
        }

        public async Task<ApiResponse<List<SopDto>>> GetAllSops(string search, string status, int page, int pageSize)
        {

            var query = _unitOfWork.Sops.GetAll();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(sop => sop.Reference.Contains(search) || sop.SopVersions.Any(sv => sv.Title.Contains(search) || sv.Description.Contains(search)));
            }

            var sops = await query
            .OrderByDescending(sop => sop.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize).Select(sop => new SopDto
            {
                Id = sop.Id,
                Reference = sop.Reference,
                DepartmentId = sop.DepartmentId,
                isAiGenerated = sop.isAiGenerated,
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
                }).ToList()
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

        public async Task<ApiResponse<SopDto>> GetLatestSopVersion(int id)
        {
            var sopEntity = await _unitOfWork.Sops.GetAsync(s => s.Id == id, includeProperties: "SopVersions,SopVersions.SopHazards,SopVersions.SopSteps,SopVersions.SopSteps.SopStepPpe");
            if (sopEntity == null)
            {
                throw new Exception("Sop not found");
            }

            var latestSopVersion = sopEntity.SopVersions
                .OrderByDescending(sv => sv.Version)
                .FirstOrDefault();

            if (latestSopVersion == null)
            {
                throw new Exception("No SopVersion found");
            }

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
                throw new Exception("Invalid id");
            }

            var sopFromDb = await _unitOfWork.Sops.GetAsync(s => s.Id == model.Id, tracked: true);
            if (sopFromDb == null)
            {
                throw new Exception("Sop not found");
            }

            // get latest sop version from db
            var latestVersion = await _unitOfWork.SopVersions.GetAll(sv => sv.SopId == model.Id, includeProperties: "SopHazards,SopSteps,SopSteps.SopStepPpe", tracked: true).OrderByDescending(sv => sv.Version).FirstOrDefaultAsync();

            if (latestVersion == null)
            {
                throw new Exception("No sop version found");
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
                        CreateDate = DateTime.UtcNow
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
                throw new Exception("No sops found");
            }


            var sopVersions = await _unitOfWork.SopVersions.GetAll(x => sopIds.Contains(x.SopId)).ToListAsync();
            var sopVersionIds = sopVersions.Select(x => x.Id).ToList();

            var sopHazards = await _unitOfWork.SopHazards.GetAll(x => sopVersionIds.Contains(x.SopVersionId)).ToListAsync();
            var sopSteps = await _unitOfWork.SopSteps.GetAll(x => sopVersionIds.Contains(x.SopVersionId)).ToListAsync();
            var sopStepIds = sopSteps.Select(x => x.Id).ToList();

            var sopStepPpe = _db.SopStepPpe.Where(x => sopStepIds.Contains(x.SopStepId)).ToList();

            var imageUrisToDelete = sopSteps.Where(x => !string.IsNullOrWhiteSpace(x.ImageUrl)).Select(x => x.ImageUrl).ToList();

            // perform deletion in order

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
                throw new Exception("Sop not found");
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
                throw new Exception("Sop is already favourited");
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
                throw new Exception("Sop favourite not found");
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

        public async Task<ApiResponse> UploadImage(FileDto file)
        {
            if (file.File == null)
            {
                throw new Exception("File is required");
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