using System.Net;
using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Repository.Interface;
using Backend.Service.Interface;

namespace Backend.Service.Implementation
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<Department>> Create(DepartmentDto model, int orgId)
        {
            validateModel(model);
            
            Department newDepartment = new Department()
                {
                    Name = model.Name,
                    OrganisationId = orgId
                };
            await _unitOfWork.Departments.AddAsync(newDepartment);
            await _unitOfWork.SaveAsync();

            return new ApiResponse<Department>()
            {
                IsSuccess = true,
                Result = newDepartment,
                StatusCode = HttpStatusCode.OK
            };
        }

        public void validateModel(DepartmentDto model)
        {
            if(string.IsNullOrWhiteSpace(model.Name))
            {
                throw new Exception("Department name must not be empty");
            }
        }

    }
}