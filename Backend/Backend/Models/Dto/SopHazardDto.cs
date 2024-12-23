using Backend.Models.DatabaseModels;

namespace Backend.Models.Dto
{
    public class SopHazardDto : BaseDto
    {
        public int? SopVersionId { get; set; }
        public string Name { get; set; }
        public string ControlMeasure { get; set; }
        public RiskLevel? RiskLevel { get; set; }
    }
}