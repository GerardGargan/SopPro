using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DatabaseModels
{
    /// <summary>
    /// The base class is used to provide default properties to models/objects such as database primary keys, and organisationId foreign keys.
    /// This class is important for global query filters which are applied in ApplicationDbContext. 
    /// A rule has been set up where any classes which inherit this base class will be automatically filtered in database queries based on the users organisation id.
    /// This helps to prevent tenancy leaks across organisations. Models/objects tied to an organisation should inherit this BaseClass.
    /// </summary>
    public abstract class BaseClass
    {
        [Key]
        public int Id { get; set; }
        public int OrganisationId { get; set; }
        [ForeignKey("OrganisationId")]
        public Organisation Organisation { get; set; }
    }
}