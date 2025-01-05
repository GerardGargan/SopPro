using Backend.Models;
using Backend.Models.Dto;

namespace Backend.Service.Interface
{
    public interface IPpeService
    {
        public Task<ApiResponse<List<PpeDto>>> GetAll();
    }
}