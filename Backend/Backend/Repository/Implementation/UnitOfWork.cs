using Backend.Data;
using Backend.Repository.Interface;

namespace Backend.Repository.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IApplicationUserRepository ApplicationUsers { get; private set; }
        public IOrganisationRepository Organisations { get; private set; }
        public IInvitationRepository Invitations { get; private set; }
        public IDepartmentRepository Departments { get; private set; }
        public IPpeRepository Ppe { get; private set; }
        public ISopRepository Sops { get; private set; }
        public ISopHazardRepository SopHazards { get; private set; }
        public ISopStepRepository SopSteps { get; private set; }
        public ISopVersionRepository SopVersions { get; private set; }
        public ISopUserFavouriteRepository SopUserFavourites { get; private set; }
        public ISettingRepository Settings { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ApplicationUsers = new ApplicationUserRepository(_db);
            Organisations = new OrganisationRepository(_db);
            Invitations = new InvitationRepository(_db);
            Departments = new DepartmentRepository(_db);
            Ppe = new PpeRepository(_db);
            Sops = new SopRepository(_db);
            SopHazards = new SopHazardRepository(_db);
            SopSteps = new SopStepRepository(_db);
            SopVersions = new SopVersionRepository(_db);
            SopUserFavourites = new SopUserFavouriteRepository(_db);
            Settings = new SettingRepository(_db);
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                await action();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var result = await action();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

    }
}
