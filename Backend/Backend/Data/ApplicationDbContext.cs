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
        public DbSet<Department> Departments { get; set; }
        public DbSet<Sop> Sops { get; set; }
        public DbSet<SopVersion> SopVersions { get; set; }
        public DbSet<SopStep> SopSteps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relationships for ApplicationUser
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Organisation)
                .WithMany(o => o.Users)
                .HasForeignKey(a => a.OrganisationId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Relationships for Invitation
            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.Organisation)
                .WithMany(o => o.Invitations)
                .HasForeignKey(i => i.OrganisationId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Relationships for Departments
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Organisation)
                .WithMany(o => o.Departments)
                .HasForeignKey(d => d.OrganisationId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            
            // Relationships for Sops
            modelBuilder.Entity<Sop>()
                .HasOne(s => s.Organisation)
                .WithMany(o => o.Sops)
                .HasForeignKey(s => s.OrganisationId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            
            modelBuilder.Entity<Sop>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Sops)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relationships for Sop Versions
            modelBuilder.Entity<SopVersion>()
                .HasOne(s => s.Organisation)
                .WithMany(o => o.SopVersions)
                .HasForeignKey(s => s.OrganisationId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            
            modelBuilder.Entity<SopVersion>()
                .HasOne(s => s.Sop)
                .WithMany(s => s.SopVersions)
                .HasForeignKey(s => s.SopId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            
            modelBuilder.Entity<SopVersion>()
                .HasOne(s => s.Author)
                .WithMany(a => a.AuthoredSopVersions)
                .HasForeignKey(s => s.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<SopVersion>()
                .HasOne(s => s.ApprovedBy)
                .WithMany(a => a.ApprovedSopVersions)
                .HasForeignKey(s => s.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationships for sop steps
            modelBuilder.Entity<SopStep>()
                .HasOne(s => s.Organisation)
                .WithMany(o => o.SopSteps)
                .HasForeignKey(s => s.OrganisationId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<SopStep>()
                .HasOne(s => s.SopVersion)
                .WithMany(s => s.SopSteps)
                .HasForeignKey(s => s.SopVersionId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
