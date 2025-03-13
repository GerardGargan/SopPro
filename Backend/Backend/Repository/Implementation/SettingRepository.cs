using Backend.Data;
using Backend.Models.DatabaseModels;
using Backend.Repository.Interface;

namespace Backend.Repository.Implementation
{
    public class SettingRepository : Repository<Setting>, ISettingRepository
    {
        private readonly ApplicationDbContext _db;
        public SettingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}