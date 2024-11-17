using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    }
}
