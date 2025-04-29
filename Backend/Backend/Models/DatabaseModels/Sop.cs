using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Backend.Models.DatabaseModels
{
    /// <summary>
    /// Sop Model
    /// </summary>
    public class Sop : BaseClass
    {
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
        public string Reference { get; set; }
        public bool? isAiGenerated { get; set; }
        public List<SopVersion> SopVersions { get; set; }
        public List<SopUserFavourite> SopUserFavourites { get; set; }
    }
}