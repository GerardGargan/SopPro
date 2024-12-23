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

                if(model.SopHazards != null && model.SopHazards.Count > 0)
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
    }
}