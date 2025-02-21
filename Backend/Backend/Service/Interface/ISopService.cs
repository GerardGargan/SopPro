using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;

namespace Backend.Service.Interface
{
    public interface ISopService
    {
        public Task<ApiResponse> CreateSop(SopDto model);
        public Task<ApiResponse<List<SopDto>>> GetAllSops(string search, int? status, int page, int pageSize, bool isFavourite, string soryBy, string sortOrder);
        public Task<ApiResponse<SopDto>> GetLatestSopVersion(int id);
        public Task<ApiResponse<SopDto>> UpdateSop(int id, SopDto model);
        public Task<ApiResponse> DeleteSops(List<int> ids);
        public Task<ApiResponse> UploadImage(FileDto file);
        public Task<ApiResponse> AddToFavourites(int id);
        public Task<ApiResponse> RemoveFromFavourites(int id);
        public Task<ApiResponse> ApproveSop(int id);
        public Task<ApiResponse> RejectSop(int id);
        public Task<ApiResponse> RequestApproval(int id);
        public Task<SopVersionDto> GetSopVersion(int sopVersionId);
        public Task<SopDto> GenerateAiSop(AiRequestDto model);
        public Task<AnalyticsResponseDto> GetAnalytics();
        public Task RevertSop(RevertRequestDto model);

    };
}