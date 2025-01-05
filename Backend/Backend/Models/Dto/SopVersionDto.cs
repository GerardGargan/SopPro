using Backend.Models.DatabaseModels;

namespace Backend.Models.Dto
{
    public class SopVersionDto : BaseDto
    {
        public int SopId { get; set; }
        public int Version { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public SopStatus Status { get; set; }
        public string AuthorId { get; set; }
        public string ApprovedById { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? RequestApprovalDate { get; set; }
        public List<SopStepDto> SopSteps { get; set; }
        public List<SopHazardDto> SopHazards { get; set; }

        public static SopVersionDto FromSopVersion(SopVersion sopVersion)
        {
            var sopVersionDto = new SopVersionDto
            {
                Id = sopVersion.Id,
                SopId = sopVersion.SopId,
                Version = sopVersion.Version,
                Title = sopVersion.Title,
                Description = sopVersion.Description,
                Status = sopVersion.Status,
                AuthorId = sopVersion.AuthorId,
                ApprovedById = sopVersion.ApprovedById,
                CreateDate = sopVersion.CreateDate,
                ApprovalDate = sopVersion.ApprovalDate,
                RequestApprovalDate = sopVersion.RequestApprovalDate,
                SopSteps = sopVersion.SopSteps?.Select(x => SopStepDto.FromSopStep(x)).ToList(),
                SopHazards = sopVersion.SopHazards?.Select(x => SopHazardDto.FromSopHazard(x)).ToList()
            };

            return sopVersionDto;
        }
    }
}