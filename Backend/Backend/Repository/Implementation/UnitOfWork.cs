using Backend.Data;
using Backend.Repository.Interface;

namespace Backend.Repository.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ApplicationUser = new ApplicationUserRepository(_db);
        }
        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
