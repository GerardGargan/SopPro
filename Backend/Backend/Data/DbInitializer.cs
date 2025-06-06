using System.Threading.Tasks;
using Backend.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    /// <summary>
    /// DbInitializer is invoked on startup (via program.cs), it will apply any migrations to the database on startup that have not been applied already.
    /// It also will seed data, such as roles if they do not exist.
    /// </summary>
    public class DbInitializer : IDbInitializer
    {
        public readonly ApplicationDbContext _db;
        public readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            // apply migrations that are not yet applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception e)
            {

            }

            // create roles if they do not exist
            if (!_roleManager.RoleExistsAsync(StaticDetails.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin)).GetAwaiter().GetResult();
            }
            if (!_roleManager.RoleExistsAsync(StaticDetails.Role_User).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_User)).GetAwaiter().GetResult();
            }

            return;
        }
    }
}