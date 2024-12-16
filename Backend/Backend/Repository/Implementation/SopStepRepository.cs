using Backend.Data;
using Backend.Models.DatabaseModels;
using Backend.Repository.Interface;

namespace Backend.Repository.Implementation {
    public class SopStepRepository : Repository<SopStep>, ISopStepRepository {
        private readonly ApplicationDbContext _db;
        public SopStepRepository(ApplicationDbContext db) : base(db) {
            _db = db;
        }
    }
}