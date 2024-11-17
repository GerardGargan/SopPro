using Backend.Data;
using Backend.Repository.Interface;

namespace Backend.Repository.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrganisationRepository Organisation { get; private set; }
        public IInvitationRepository Invitation { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ApplicationUser = new ApplicationUserRepository(_db);
            Organisation = new OrganisationRepository(_db);
            Invitation = new InvitationRepository(_db);
        }
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
