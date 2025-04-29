using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DatabaseModels
{
    /// <summary>
    /// SOP User Favourite Model
    /// </summary>
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