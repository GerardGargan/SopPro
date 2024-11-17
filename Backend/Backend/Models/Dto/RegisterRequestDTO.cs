using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Dto
{
    public class RegisterRequestDTO
    {
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Forename { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
