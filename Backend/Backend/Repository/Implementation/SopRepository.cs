using Backend.Data;
using Backend.Models.DatabaseModels;
using Backend.Repository.Interface;

namespace Backend.Repository.Implementation {
    public class SopRepository : Repository<Sop>, ISopRepository {
        private readonly ApplicationDbContext _db;
        public SopRepository(ApplicationDbContext db) : base(db) {
            _db = db;
        }
    }
}