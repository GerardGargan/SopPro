using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DatabaseModels {
    public class SopStepPpe : BaseOrganisationClass {
        [Key]
        public int Id { get; set; }
        public int SopStepId { get; set; }
        [ForeignKey("SopStepId")]
        public SopStep SopStep { get; set; }
        public int PpeId { get; set; }
        [ForeignKey("PpeId")]
        public Ppe Ppe { get; set; }
    }
}