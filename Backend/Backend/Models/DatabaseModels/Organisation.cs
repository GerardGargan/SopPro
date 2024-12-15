using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DatabaseModels
{
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
    }
}
