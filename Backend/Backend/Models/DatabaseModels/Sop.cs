using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Backend.Models.DatabaseModels {
    public class Sop : BaseOrganisationClass {
        [Key]
        public int Id { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
        public string reference { get; set; }
        public bool isAiGenerated { get; set; }
        public List<SopVersion> SopVersions { get; set; }
    }
}