using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DatabaseModels
{
    /// <summary>
    /// Setting model
    /// </summary>
    public class Setting : BaseClass
    {
        public string Type { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

    }
}