namespace Backend.Models.Dto {
    public class SopDto : BaseDto {
        public int? DepartmentId { get; set; }
        public string Reference { get; set; }
        public bool? isAiGenerated { get; set; }
        public List<SopHazardDto> SopHazards { get; set; }
        public List<SopVersionDto> SopVersions { get; set; }
    }
}