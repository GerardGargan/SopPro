using System.Net;
using Backend.Data;
using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Models.Tenancy;
using Backend.Repository.Interface;
using Backend.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service.Implementation
{
    public class SopService : ISopService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenancyResolver _tenancyResolver;
        private readonly ApplicationDbContext _db;

        public SopService(IUnitOfWork unitOfWork, ITenancyResolver tenancyResolver, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _tenancyResolver = tenancyResolver;
            _db = db;
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
                    await _unitOfWork.SaveAsync();
                }
            });

            return new ApiResponse
            {
                IsSuccess = true,
                SuccessMessage = "Sop created successfully"
            };
        }

        public async Task<ApiResponse<List<SopDto>>> GetAllSops()
        {
            List<SopDto> sops = await _unitOfWork.Sops.GetAll().Select(sop => new SopDto
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
                    }).ToList()
                }).ToList()
            }).ToListAsync();


            return new ApiResponse<List<SopDto>>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = sops
            };
        }

        public async Task<ApiResponse<SopDto>> GetLatestSopVersion(int id)
        {
            var sopEntity = await _unitOfWork.Sops.GetAsync(s => s.Id == id, includeProperties: "SopVersions,SopVersions.SopHazards,SopVersions.SopSteps");
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
                SopSteps = latestSopVersion.SopSteps.Select(sopStep => new SopStep
                {
                    Id = sopStep.Id,
                    SopVersionId = sopStep.SopVersionId,
                    Position = sopStep.Position,
                    Text = sopStep.Text,
                    Title = sopStep.Title,
                    ImageUrl = sopStep.ImageUrl,
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

            var sopFromDb = await _unitOfWork.Sops.GetAsync(s => s.Id == model.Id);
            if (sopFromDb == null)
            {
                throw new Exception("Sop not found");
            }

            // get latest sop version from db
            var latestVersion = await _unitOfWork.SopVersions.GetAll(sv => sv.SopId == model.Id, includeProperties: "SopHazards,SopSteps", tracked: true).OrderByDescending(sv => sv.Version).FirstOrDefaultAsync();

            if (latestVersion == null)
            {
                throw new Exception("No sop version found");
            }

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
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

                    // duplicate sop steps
                    List<SopStep> sopSteps = DuplicateSteps(latestVersion, newVersion.Id);
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
                    await _unitOfWork.SaveAsync();

                    var updatedSop = await _unitOfWork.Sops.GetAsync(s => s.Id == model.Id, includeProperties: "SopVersions,SopVersions.SopHazards");

                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Result = SopDto.FromSop(updatedSop);

                }
            });

            return response;

        }

        public async Task<ApiResponse> DeleteSop(int id)
        {
            var response = new ApiResponse();

            var sop = await _unitOfWork.Sops.GetAsync(x => x.Id == id);

            if (sop == null)
            {
                throw new Exception("Sop not found");
            }

            var sopVersions = await _unitOfWork.SopVersions.GetAll(x => x.SopId == id).ToListAsync();
            var sopVersionIds = sopVersions.Select(x => x.Id).ToList();

            var sopHazards = await _unitOfWork.SopHazards.GetAll(x => sopVersionIds.Contains(x.SopVersionId)).ToListAsync();
            var sopSteps = await _unitOfWork.SopSteps.GetAll(x => sopVersionIds.Contains(x.SopVersionId)).ToListAsync();
            var sopStepIds = sopSteps.Select(x => x.Id).ToList();

            var sopStepPpe = _db.SopStepPpe.Where(x => sopStepIds.Contains(x.SopStepId)).ToList();

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
                _unitOfWork.Sops.Remove(sop);

                await _unitOfWork.SaveAsync();

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.SuccessMessage = $"Sop id: {id} deleted successfully";
            });

            return response;
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