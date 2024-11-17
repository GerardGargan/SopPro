using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Dto
{
    public class RegisterInviteRequestDTO
    {
        [Required]
        public string Forename { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
