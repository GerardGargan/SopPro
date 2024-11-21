using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Dto
{
    public class OrganisationSignupRequest
    {
        [Required]
        public string OrganisationName { get; set; }
        [Required]
        public string Forename { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
