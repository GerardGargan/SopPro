using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DatabaseModels
{
    /// <summary>
    /// Organisation model
    /// </summary>
    public class Organisation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public List<Invitation> Invitations { get; set; }
        public List<Department> Departments { get; set; }
        public List<Sop> Sops { get; set; }
        public List<SopVersion> SopVersions { get; set; }
        public List<SopStep> SopSteps { get; set; }
        public List<SopStepPpe> SopStepPpe { get; set; }
        public List<SopHazard> SopHazards { get; set; }
        public List<SopUserFavourite> SopUserFavourites { get; set; }
        public List<Setting> Settings { get; set; }
    }
}
