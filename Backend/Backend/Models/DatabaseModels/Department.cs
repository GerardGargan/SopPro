
namespace Backend.Models.DatabaseModels
{
    /// <summary>
    /// Model for department
    /// </summary>
    public class Department : BaseClass
    {
        public string Name { get; set; }
        public List<Sop> Sops { get; set; }
    }
}