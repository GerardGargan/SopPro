using Backend.Data;
using Backend.Models.DatabaseModels;
using Backend.Repository.Interface;

namespace Backend.Repository.Implementation {
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository {
        private readonly ApplicationDbContext _db;

        public DepartmentRepository(ApplicationDbContext db) : base(db) {
            _db = db;
        }
    }
}