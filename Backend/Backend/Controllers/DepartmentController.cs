
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
    }
}