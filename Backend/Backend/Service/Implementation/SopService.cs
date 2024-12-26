using System.Net;
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

        public SopService(IUnitOfWork unitOfWork, ITenancyResolver tenancyResolver)
        {
            _unitOfWork = unitOfWork;
            _tenancyResolver = tenancyResolver;
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

                var firstSopVersion = model.SopVersions.FirstOrDefault();
                if (firstSopVersion == null)
                {
                    throw new Exception("Sop version is required");
                }

                // Create sop version record
                var sopVersion = new SopVersion
                {
                    SopId = sop.Id,
                    Version = 1,
                    AuthorId = _tenancyResolver.GetUserId(),
                    CreateDate = DateTime.UtcNow,
                    Title = firstSopVersion.Title,
                    Description = firstSopVersion.Description,
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
            var sopEntity = await _unitOfWork.Sops.GetAsync(s => s.Id == id, includeProperties: "SopVersions,SopVersions.SopHazards");
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
                DepartmentId = sopEntity.DepartmentId ?? 0,
                isAiGenerated = sopEntity.isAiGenerated,
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
        public async Task<ApiResponse<Sop>> UpdateSop(int id, SopDto model)
        {
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

                    // create new version
                    var newVersion = new SopVersion
                    {
                        SopId = sopFromDb.Id,
                        Version = latestVersion.Version + 1,
                        AuthorId = _tenancyResolver.GetUserId(),
                        CreateDate = DateTime.UtcNow,
                        Title = model.SopVersions.FirstOrDefault().Title,
                        Description = model.SopVersions.FirstOrDefault().Description,
                        Status = SopStatus.Draft,
                        OrganisationId = _tenancyResolver.GetOrganisationid().Value,
                        SopHazards = model.SopHazards.Select(sopHazard => new SopHazard
                        {
                            Name = sopHazard.Name,
                            ControlMeasure = sopHazard.ControlMeasure,
                            RiskLevel = sopHazard.RiskLevel,
                            OrganisationId = _tenancyResolver.GetOrganisationid().Value
                        }).ToList()
                    };

                    await _unitOfWork.SopVersions.AddAsync(newVersion);
                    await _unitOfWork.SaveAsync();

                    // Duplicate sop steps and link to new vesion
                    var newSopSteps = latestVersion.SopSteps.Select(sopStep => new SopStep
                    {
                        SopVersionId = newVersion.Id,
                        Position = sopStep.Position,
                        Text = sopStep.Text,
                        ImageUrl = sopStep.ImageUrl,
                        SopStepPpe = sopStep.SopStepPpe.Select(sopStepPpe => new SopStepPpe
                        {
                            SopStepId = sopStepPpe.SopStepId,
                            PpeId = sopStepPpe.PpeId
                        }).ToList()
                    });

                    await _unitOfWork.SopSteps.AddRangeAsync(newSopSteps);
                    await _unitOfWork.SaveAsync();

                }
                else
                {

                    // update existing sop version
                    latestVersion.Title = model.SopVersions.FirstOrDefault().Title;
                    latestVersion.Description = model.SopVersions.FirstOrDefault().Description;

                    // update sop hazards
                    var existingHazardIds = latestVersion.SopHazards.Select(h => h.Id).ToList();
                    var modelHazardIds = model.SopHazards.Select(h => h.Id).ToList();

                    var hazardsToDelete = existingHazardIds.Where(id => !modelHazardIds.Contains(id)).ToList();
                    var hazardsToUpdate = existingHazardIds.Where(id => modelHazardIds.Contains(id)).ToList();
                    var hazardsToAdd = modelHazardIds.Where(id => id == null).ToList();

                    // delete hazards
                    if (hazardsToDelete != null && hazardsToDelete.Count > 0)
                    {
                        latestVersion.SopHazards.RemoveAll(h => hazardsToDelete.Contains(h.Id));
                    }

                    // update hazards
                    if (hazardsToUpdate != null && hazardsToUpdate.Count > 0)
                    {
                        foreach (var hazardId in hazardsToUpdate)
                        {
                            var hazard = latestVersion.SopHazards.FirstOrDefault(h => h.Id == hazardId);
                            var modelHazard = model.SopHazards.FirstOrDefault(h => h.Id == hazardId);

                            hazard.Name = modelHazard.Name;
                            hazard.ControlMeasure = modelHazard.ControlMeasure;
                            hazard.RiskLevel = modelHazard.RiskLevel;
                        }
                    }

                    // add hazards
                    if (hazardsToAdd != null && hazardsToAdd.Count > 0)
                    {
                        var newHazards = model.SopHazards.Where(h => h.Id == null).Select(h => new SopHazard
                        {
                            SopVersionId = latestVersion.Id,
                            Name = h.Name,
                            ControlMeasure = h.ControlMeasure,
                            RiskLevel = h.RiskLevel,
                            OrganisationId = _tenancyResolver.GetOrganisationid().Value
                        }).ToList();

                        await _unitOfWork.SopHazards.AddRangeAsync(newHazards);
                    }
                    await _unitOfWork.SaveAsync();

                }

            });

            var updatedSop = await _unitOfWork.Sops.GetAsync(s => s.Id == model.Id, includeProperties: "SopVersions, SopVersions.SopHazards");
            var updatedSopVersion = updatedSop.SopVersions.OrderByDescending(sv => sv.Version).FirstOrDefault();
            updatedSop.SopVersions = new List<SopVersion> { updatedSopVersion };

            return new ApiResponse<Sop>
            {
                IsSuccess = true,
                SuccessMessage = "Sop updated successfully",
                StatusCode = HttpStatusCode.OK,
                Result = updatedSop
            };
        }
    }
}