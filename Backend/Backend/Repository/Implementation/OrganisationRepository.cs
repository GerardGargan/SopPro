using Backend.Data;
using Backend.Models.DatabaseModels;
using Backend.Repository.Interface;

namespace Backend.Repository.Implementation
{
    public class OrganisationRepository : Repository<Organisation>, IOrganisationRepository
    {
        private readonly ApplicationDbContext _db;
        public OrganisationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
