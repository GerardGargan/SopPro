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

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ApplicationUsers = new ApplicationUserRepository(_db);
            Organisations = new OrganisationRepository(_db);
            Invitations = new InvitationRepository(_db);
        }
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
