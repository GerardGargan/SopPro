using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;

namespace Backend.Service.Interface
{
    public interface ISopService
    {
        public Task<ApiResponse> CreateSop(SopDto model);
        public Task<ApiResponse<List<SopDto>>> GetAllSops();
        public Task<ApiResponse<SopDto>> GetLatestSopVersion(int id);
        public Task<ApiResponse<SopDto>> UpdateSop(int id, SopDto model);
    };
}