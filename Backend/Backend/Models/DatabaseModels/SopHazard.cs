using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DatabaseModels {
    public class SopHazard : BaseOrganisationClass {
        [Key]
        public int Id { get; set; }
        public int SopVersionId { get; set; }
        [ForeignKey("SopVersionId")]
        public SopVersion SopVersion { get; set; }
        public string Name { get; set; }
        public string ControlMeasure { get; set; }
        public RiskLevel? RiskLevel { get; set; }
        
    }

    public enum RiskLevel {
        Low = 1,
        Medium = 2,
        High = 3
    }
}