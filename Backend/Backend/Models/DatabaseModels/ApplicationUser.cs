using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DatabaseModels
{
    public class ApplicationUser : IdentityUser
    {
        public string Forename { get; set; }
        public string Surname { get; set; }
        public int OrganisationId { get; set; }
        [ForeignKey("OrganisationId")]
        public Organisation Organisation { get; set; }
        public List<SopVersion> AuthoredSopVersions { get; set; }
        public List<SopVersion> ApprovedSopVersions { get; set; }
    }
}
