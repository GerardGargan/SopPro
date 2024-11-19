using Backend.Data;
using Backend.Repository.Interface;
using Microsoft.EntityFrameworkCore.Storage;

namespace Backend.Repository.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IApplicationUserRepository ApplicationUsers { get; private set; }
        public IOrganisationRepository Organisations { get; private set; }
        public IInvitationRepository Invitations { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ApplicationUsers = new ApplicationUserRepository(_db);
            Organisations = new OrganisationRepository(_db);
            Invitations = new InvitationRepository(_db);
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
