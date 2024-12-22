using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DatabaseModels {
    public class Department : BaseClass {
        public string Name { get; set; }
        public List<Sop> Sops { get; set; }
    }
}