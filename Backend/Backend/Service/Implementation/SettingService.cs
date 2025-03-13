using Backend.Models.Dto;
using Backend.Repository.Interface;
using Backend.Service.Interface;

namespace Backend.Service.Implementation
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SettingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Create(SettingDto model)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<SettingDto> GetSettingByKey(string key)
        {
            throw new NotImplementedException();
        }

        public async Task Update(SettingDto model)
        {
            throw new NotImplementedException();
        }
    }
}