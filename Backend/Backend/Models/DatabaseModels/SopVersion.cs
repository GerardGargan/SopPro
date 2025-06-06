using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DatabaseModels
{
    /// <summary>
    /// SOP Version Model
    /// </summary>
    public class SopVersion : BaseClass
    {
        public int SopId { get; set; }
        [ForeignKey("SopId")]
        public Sop Sop { get; set; }
        public int Version { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? LastUpdated { get; set; }
        public SopStatus Status { get; set; }
        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public ApplicationUser Author { get; set; }
        public string ApprovedById { get; set; }
        [ForeignKey("ApprovedById")]
        public ApplicationUser ApprovedBy { get; set; }
        public DateTime? CreateDate { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovalDate { get; set; }
        public DateTime? RequestApprovalDate { get; set; }
        public List<SopStep> SopSteps { get; set; }
        public List<SopHazard> SopHazards { get; set; }
    }

    public enum SopStatus
    {
        Draft = 1,
        InReview = 2,
        Approved = 3,
        Archived = 4,
        Rejected = 5

    }
}