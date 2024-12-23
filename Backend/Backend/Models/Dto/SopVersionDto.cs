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
        
        // Collections for related entities
        // TODO: Implement these
        // public List<SopStepDto> SopSteps { get; set; }
        // public List<SopHazardDto> SopHazards { get; set; }
    }
}