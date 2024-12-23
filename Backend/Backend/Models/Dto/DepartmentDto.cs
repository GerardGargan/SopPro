namespace Backend.Models.Dto
{
    public class DepartmentDto : BaseDto
    {
        public string Name { get; set; }
        public List<SopDto> Sops { get; set; }
    }
}