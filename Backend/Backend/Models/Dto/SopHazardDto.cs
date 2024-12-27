using Backend.Models.DatabaseModels;

namespace Backend.Models.Dto
{
    public class SopHazardDto : BaseDto
    {
        public int? SopVersionId { get; set; }
        public string Name { get; set; }
        public string ControlMeasure { get; set; }
        public RiskLevel? RiskLevel { get; set; }

        public static SopHazardDto FromSopHazard(SopHazard sopHazard)
        {
            var sopHazardDto = new SopHazardDto
            {
                Id = sopHazard.Id,
                SopVersionId = sopHazard.SopVersionId,
                Name = sopHazard.Name,
                ControlMeasure = sopHazard.ControlMeasure,
                RiskLevel = sopHazard.RiskLevel
            };

            return sopHazardDto;
        }
    }
}