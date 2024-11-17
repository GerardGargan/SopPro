using Backend.Data;
using Backend.Models.DatabaseModels;
using Backend.Repository.Interface;

namespace Backend.Repository.Implementation
{
    public class InvitationRepository : Repository<Invitation>, IInvitationRepository
    {
        private readonly ApplicationDbContext _db;
        public InvitationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
