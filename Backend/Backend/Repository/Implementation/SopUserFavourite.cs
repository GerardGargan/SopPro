using Backend.Data;
using Backend.Models.DatabaseModels;
using Backend.Repository.Interface;

namespace Backend.Repository.Implementation
{
    public class SopUserFavouriteRepository : Repository<SopUserFavourite>, ISopUserFavouriteRepository
    {
        private readonly ApplicationDbContext _db;
        public SopUserFavouriteRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}