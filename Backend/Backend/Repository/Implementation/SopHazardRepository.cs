using Backend.Data;
using Backend.Models.DatabaseModels;
using Backend.Repository.Interface;

namespace Backend.Repository.Implementation 
{
    public class SopHazardRepository : Repository<SopHazard>, ISopHazardRepository {
        private readonly ApplicationDbContext _db;
        public SopHazardRepository(ApplicationDbContext db) : base(db) {
            _db = db;
        }
    }
}