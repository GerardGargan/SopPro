using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DatabaseModels {
    public class Department : BaseOrganisationClass {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}