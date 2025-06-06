﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DatabaseModels
{
    /// <summary>
    /// Database model for Application Users. Extends the default IdentityUser with additional properties and relationships.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string Forename { get; set; }
        public string Surname { get; set; }
        public int OrganisationId { get; set; }
        [ForeignKey("OrganisationId")]
        public Organisation Organisation { get; set; }
        public List<SopVersion> AuthoredSopVersions { get; set; }
        public List<SopVersion> ApprovedSopVersions { get; set; }
        public List<SopUserFavourite> SopUserFavourites { get; set; }
        public List<Setting> Settings { get; set; }
    }
}
