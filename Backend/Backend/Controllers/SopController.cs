using Backend.Models.Dto;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers {
    [Route("api/sop")]
    [ApiController]
    [Authorize]
    public class SopController : BaseApiController 
    {
        private readonly ISopService _sopService;

        public SopController(ISopService sopService) {

            _sopService = sopService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSop([FromBody] SopDto sopDto)
        {
            var apiResponse = await _sopService.CreateSop(sopDto);
            return Ok(apiResponse);
        }
    }
}