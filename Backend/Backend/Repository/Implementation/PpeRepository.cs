using Backend.Data;
using Backend.Models.DatabaseModels;
using Backend.Repository.Interface;

namespace Backend.Repository.Implementation {
    public class PpeRepository : Repository<Ppe>, IPpeRepository {
        private readonly ApplicationDbContext _db;
        public PpeRepository(ApplicationDbContext db) : base(db) {
            _db = db;
        }
    }
}