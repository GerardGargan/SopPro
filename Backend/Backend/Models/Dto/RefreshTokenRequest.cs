using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Dto
{
    public class RefreshTokenRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}