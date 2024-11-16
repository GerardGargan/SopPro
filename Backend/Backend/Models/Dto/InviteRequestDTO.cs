using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Dto
{
    public class InviteRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string Organisation { get; set; }
    }
}
