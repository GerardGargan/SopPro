namespace Backend.Models.Dto
{
    public class DepartmentDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public List<SopDto> Sops { get; set; }
    }
}