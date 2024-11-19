using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Dto
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
