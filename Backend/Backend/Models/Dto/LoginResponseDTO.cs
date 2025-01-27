namespace Backend.Models.Dto
{
    public class LoginResponseDTO
    {
        public string Email { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
