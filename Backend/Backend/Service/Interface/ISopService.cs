using Backend.Models;
using Backend.Models.Dto;

namespace Backend.Service.Interface {
    public interface ISopService {
        public Task<ApiResponse> CreateSop(SopDto model);
    };
}