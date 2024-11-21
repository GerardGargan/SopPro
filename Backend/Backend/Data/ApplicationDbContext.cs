using Backend.Models.DatabaseModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Invitation> Invitations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relationships for ApplicationUser
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Organisation)
                .WithMany(o => o.Users)
                .HasForeignKey(a => a.OrganisationId)
                .IsRequired();

            // Relationships for Invitation
            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.Organisation)
                .WithMany(o => o.Invitations)
                .HasForeignKey(i => i.OrganisationId)
                .IsRequired();
        }

        
    }
}
