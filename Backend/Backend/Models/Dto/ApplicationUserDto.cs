namespace Backend.Models.Dto
{
    public class ApplicationUserDto
    {
        public string Id { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public int? OrganisationId { get; set; }
        public string RoleId { get; set; }
    }
}