using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Dto
{
    public class RegisterRequestDTO
    {
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
