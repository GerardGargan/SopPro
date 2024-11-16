using Microsoft.AspNetCore.Identity;

namespace Backend.Models.DatabaseModels
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
