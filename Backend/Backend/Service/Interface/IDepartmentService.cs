using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;

namespace Backend.Service.Interface {
    public interface IDepartmentService
    {
        public Task<ApiResponse<Department>> Create(DepartmentDto model, int orgId);
        public Task<ApiResponse<Department>> Update(DepartmentDto model);
        public Task<ApiResponse<List<Department>>> GetAll();
        public Task<ApiResponse<Department>> GetById(int id);

    }
}