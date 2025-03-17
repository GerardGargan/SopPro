using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Models.Tenancy;
using Backend.Repository.Interface;
using Backend.Service.Interface;

namespace Backend.Service.Implementation
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenancyResolver _tenancyResolver;

        public SettingService(IUnitOfWork unitOfWork, ITenancyResolver tenancyResolver)
        {
            _unitOfWork = unitOfWork;
            _tenancyResolver = tenancyResolver;
        }

        public async Task Create(SettingDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Type))
            {
                throw new ArgumentException("Type cant be empty");
            }

            if (string.IsNullOrWhiteSpace(model.Key))
            {
                throw new ArgumentException("Key cant be empty");
            }

            if (string.IsNullOrWhiteSpace(model.Value))
            {
                throw new ArgumentException("Value cant be empty");
            }

            // Check if it already exists
            Setting settingFromDb = await _unitOfWork.Settings.GetAsync(x => x.Key.ToLower() == model.Key.ToLower());
            if (settingFromDb != null)
            {
                throw new Exception("Setting key already exists, unable to create");
            }

            Setting setting = new Setting()
            {
                Key = model.Key,
                Type = model.Type,
                ApplicationUserId = model.ApplicationUserId,
                OrganisationId = _tenancyResolver.GetOrganisationid().Value
            };

            await _unitOfWork.Settings.AddAsync(setting);
            await _unitOfWork.SaveAsync();
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