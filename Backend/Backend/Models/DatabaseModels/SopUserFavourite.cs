using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DatabaseModels
{
    public class SopUserFavourite : BaseClass
    {
        public int SopId { get; set; }
        [ForeignKey("SopId")]
        public Sop Sop { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}