using Backend.Models.Dto;

namespace Backend.Service.Interface
{
    public interface ISettingService
    {
        Task<SettingDto> GetSettingByKey(string key);
        Task Create(SettingDto model);
        Task Update(SettingDto model);
        Task Delete(int id);
    }
}