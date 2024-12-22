using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DatabaseModels {
    public class SopStep : BaseClass {
        public int SopVersionId { get; set; }
        [ForeignKey("SopVersionId")]
        public SopVersion SopVersion { get; set; }
        public int? Position { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public List<SopStepPpe> SopStepPpe { get; set; }
    }
}