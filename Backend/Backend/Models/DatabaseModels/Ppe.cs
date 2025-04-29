using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DatabaseModels
{
    /// <summary>
    /// PPE Model
    /// </summary>
    public class Ppe
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public List<SopStepPpe> SopStepPpe { get; set; }
    }
}