
using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers {
    [Route("api/department")]
    [ApiController]

    [Authorize]
    public class DepartmentController : BaseApiController 
    {
        private readonly IDepartmentService _departmentService;
        public DepartmentController (IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Department>))]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentDto model)
        {
            var orgIdClaim = User.FindFirst("organisationId")?.Value;

            if(!int.TryParse(orgIdClaim, out int orgId))
            {
                throw new Exception("Invalid organisationId.");
            }

            var apiResponse = await _departmentService.Create(model, orgId);
            return Ok(apiResponse);
        }

        [HttpPut]
        [Route("{id:int}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Department>))]
        public async Task<IActionResult> UpdateDepartment([FromBody] DepartmentDto model)
        {
            var apiResponse = await _departmentService.Update(model);

            return Ok(apiResponse);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponse<List<Department>>))]
        public async Task<IActionResult> GetAll()
        {
            var apiResponse = await _departmentService.GetAll();

            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Department>))]
        public async Task<IActionResult> GetById(int id)
        {
            var apiResponse = await _departmentService.GetById(id);
            return Ok(apiResponse);
        }
    }
}