using System.Net;
using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Repository.Interface;
using Backend.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service.Implementation
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Creates a department
        /// </summary>
        /// <param name="model"></param>
        /// <param name="orgId"></param>
        /// <returns>A Department DTO object for the department that was created</returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse<Department>> Create(DepartmentDto model, int orgId)
        {
            validateModel(model);

            // Check for duplicate departments
            var duplicateDepartment = await _unitOfWork.Departments.GetAsync(d => d.Name.ToLower().Trim() == model.Name.ToLower().Trim() && d.OrganisationId == orgId);
            if (duplicateDepartment != null)
            {
                throw new Exception("This department already exists");
            }

            // Create department
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

        /// <summary>
        /// Updates a department
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse<Department>> Update(DepartmentDto model)
        {
            if (model.Id == null)
            {
                throw new Exception("Id is required");
            }

            // Check is the department exists
            Department deptToUpdate = await _unitOfWork.Departments.GetAsync(d => d.Id == model.Id, tracked: true);

            if (deptToUpdate == null)
            {
                throw new Exception("Department does not exist");
            }

            validateModel(model);

            // Update properties
            deptToUpdate.Name = model.Name;
            await _unitOfWork.SaveAsync();

            return new ApiResponse<Department>
            {
                IsSuccess = true,
                SuccessMessage = "Department successfully updated",
                Result = deptToUpdate,
                StatusCode = HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Returns a list of all departments
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse<List<DepartmentDto>>> GetAll()
        {
            // Get all departments
            List<DepartmentDto> departments = await _unitOfWork.Departments.GetAll().Select(dept => new DepartmentDto
            {
                Id = dept.Id,
                Name = dept.Name
            }).ToListAsync();

            var result = new ApiResponse<List<DepartmentDto>>
            {
                IsSuccess = true,
                Result = departments,
                StatusCode = HttpStatusCode.OK
            };

            return result;
        }

        /// <summary>
        /// Gets a department by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse<Department>> GetById(int id)
        {
            if (id <= 0)
            {
                throw new Exception("Invalid id");
            }

            // Fetch department
            Department deptFromDb = await _unitOfWork.Departments.GetAsync(d => d.Id == id);

            if (deptFromDb == null)
            {
                throw new Exception("Department does not exist");
            }

            return new ApiResponse<Department>
            {
                IsSuccess = true,
                Result = deptFromDb,
                StatusCode = HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Deletes a department by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse> Delete(int id)
        {
            // obtain department record from db
            var deptFromDb = await _unitOfWork.Departments.GetAsync(d => d.Id == id, tracked: true);

            if (deptFromDb == null)
            {
                throw new Exception("Department does not exist");
            }

            _unitOfWork.Departments.Remove(deptFromDb);
            await _unitOfWork.SaveAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                SuccessMessage = "Department successfully deleted",
                StatusCode = HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Validates the department dto model for business rules
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="Exception"></exception>
        public void validateModel(DepartmentDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new Exception("Department name must not be empty");
            }
        }
    }
}