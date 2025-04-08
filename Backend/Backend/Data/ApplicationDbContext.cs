using System.Reflection;
using Backend.Models.DatabaseModels;
using Backend.Models.Tenancy;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenancyResolver _tenancyResolver;
        public ApplicationDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor, ITenancyResolver tenancyResolver) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenancyResolver = tenancyResolver;
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Sop> Sops { get; set; }
        public DbSet<SopVersion> SopVersions { get; set; }
        public DbSet<SopStep> SopSteps { get; set; }
        public DbSet<Ppe> Ppe { get; set; }
        public DbSet<SopStepPpe> SopStepPpe { get; set; }
        public DbSet<SopHazard> SopHazards { get; set; }
        public DbSet<SopUserFavourite> SopUserFavourites { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Loop through each entity type that inherits from BaseClass and apply a global query filter for organisation, this helps to avoid tenancy leaks
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseClass).IsAssignableFrom(entityType.ClrType) && entityType.ClrType != typeof(BaseClass))
                {
                    var method = typeof(ModelBuilderExtensions)
                        .GetMethod(nameof(ModelBuilderExtensions.ApplyOrganisationQueryFilter), BindingFlags.Static | BindingFlags.Public)
                        .MakeGenericMethod(entityType.ClrType);

                    method.Invoke(null, new object[] { modelBuilder, this });
                }
            }

            // Set up global query filter for users/organisation. Users don't inherit from BaseClass so needs to be handled separately to the above.
            modelBuilder.Entity<ApplicationUser>()
                .HasQueryFilter(u => u.OrganisationId == _tenancyResolver.GetOrganisationid());

            // Relationships for ApplicationUser
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
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Relationships for Departments
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Organisation)
                .WithMany(o => o.Departments)
                .HasForeignKey(d => d.OrganisationId)
                .OnDelete(DeleteBehavior.Restrict)
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
                .OnDelete(DeleteBehavior.Restrict)
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
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Relationships for SopStepPpe
            modelBuilder.Entity<SopStepPpe>()
                .HasOne(s => s.Organisation)
                .WithMany(o => o.SopStepPpe)
                .HasForeignKey(s => s.OrganisationId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<SopStepPpe>()
                .HasOne(s => s.SopStep)
                .WithMany(s => s.SopStepPpe)
                .HasForeignKey(s => s.SopStepId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<SopStepPpe>()
                .HasOne(s => s.Ppe)
                .WithMany(s => s.SopStepPpe)
                .HasForeignKey(s => s.PpeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Relationships for Hazards
            modelBuilder.Entity<SopHazard>()
                .HasOne(h => h.Organisation)
                .WithMany(o => o.SopHazards)
                .HasForeignKey(h => h.OrganisationId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<SopHazard>()
                .HasOne(h => h.SopVersion)
                .WithMany(s => s.SopHazards)
                .HasForeignKey(h => h.SopVersionId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Relationships for SopUserFavourites
            modelBuilder.Entity<SopUserFavourite>()
                .HasOne(s => s.Organisation)
                .WithMany(o => o.SopUserFavourites)
                .HasForeignKey(s => s.OrganisationId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<SopUserFavourite>()
                .HasOne(s => s.Sop)
                .WithMany(s => s.SopUserFavourites)
                .HasForeignKey(s => s.SopId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<SopUserFavourite>()
                .HasOne(s => s.ApplicationUser)
                .WithMany(s => s.SopUserFavourites)
                .HasForeignKey(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Setting>()
                .HasOne(s => s.Organisation)
                .WithMany(s => s.Settings)
                .HasForeignKey(s => s.OrganisationId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Setting>()
                .HasOne(s => s.ApplicationUser)
                .WithMany(s => s.Settings)
                .HasForeignKey(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// Returns the users organisation id, if none exists (e.g. if user is not logged in) -1 is returns
        /// </summary>
        public int CurrentOrganisationId
        {
            get
            {
                if (_httpContextAccessor == null || _httpContextAccessor.HttpContext == null)
                {
                    return -1;
                }

                var user = _httpContextAccessor.HttpContext.User;
                var organisationClaim = user?.FindFirst("organisationId");

                return organisationClaim != null ? int.Parse(organisationClaim.Value) : -1;
            }
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void ApplyOrganisationQueryFilter<T>(this ModelBuilder builder, ApplicationDbContext context)
            where T : BaseClass
        {
            builder.Entity<T>().HasQueryFilter(e => e.OrganisationId == context.CurrentOrganisationId);
        }
    }
}
