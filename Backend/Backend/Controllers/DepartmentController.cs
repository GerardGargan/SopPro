
using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/department")]
    [ApiController]

    [Authorize]
    public class DepartmentController : BaseApiController
    {
        private readonly IDepartmentService _departmentService;
        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        /// Creates a department
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Department>))]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentDto model)
        {
            var orgIdClaim = User.FindFirst("organisationId")?.Value;

            if (!int.TryParse(orgIdClaim, out int orgId))
            {
                throw new Exception("Invalid organisationId.");
            }

            var apiResponse = await _departmentService.Create(model, orgId);
            return Ok(apiResponse);
        }

        /// <summary>
        /// Updates a department
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:int}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Department>))]
        public async Task<IActionResult> UpdateDepartment([FromBody] DepartmentDto model)
        {
            var apiResponse = await _departmentService.Update(model);

            return Ok(apiResponse);
        }

        /// <summary>
        /// Gets all departments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponse<List<Department>>))]
        public async Task<IActionResult> GetAll()
        {
            var apiResponse = await _departmentService.GetAll();

            return Ok(apiResponse);
        }

        /// <summary>
        /// Gets a department by their id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Department>))]
        public async Task<IActionResult> GetById(int id)
        {
            var apiResponse = await _departmentService.GetById(id);
            return Ok(apiResponse);
        }

        /// <summary>
        /// Delete a department by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:int}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Delete(int id)
        {
            var apiResponse = await _departmentService.Delete(id);
            return Ok(apiResponse);
        }
    }
}