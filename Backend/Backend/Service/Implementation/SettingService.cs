using Backend.Models.DatabaseModels;
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
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key was not provided");
            }

            Setting setting = await _unitOfWork.Settings.GetAsync(x => x.Key.ToLower() == key.ToLower());

            if (setting == null)
            {
                throw new KeyNotFoundException("Setting not found");
            }

            SettingDto settingDto = new SettingDto()
            {
                Id = setting.Id,
                Type = setting.Type,
                Key = setting.Key,
                ApplicationUserId = setting.ApplicationUserId,
                Value = setting.Value
            };

            return settingDto;
        }

        public async Task Update(SettingDto model)
        {
            throw new NotImplementedException();
        }
    }
}