using Backend.Data;
using Backend.Models.DatabaseModels;
using Backend.Repository.Interface;

namespace Backend.Repository.Implementation {
    public class SopVersionRepository : Repository<SopVersion>, ISopVersionRepository {
        private readonly ApplicationDbContext _db;
        public SopVersionRepository(ApplicationDbContext db) : base(db) {
            _db = db;
        }
    }
}