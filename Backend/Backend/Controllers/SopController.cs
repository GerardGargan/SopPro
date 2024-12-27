using Backend.Models;
using Backend.Models.Dto;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/sop")]
    [ApiController]
    [Authorize]
    public class SopController : BaseApiController
    {
        private readonly ISopService _sopService;

        public SopController(ISopService sopService)
        {

            _sopService = sopService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSop([FromBody] SopDto sopDto)
        {
            var apiResponse = await _sopService.CreateSop(sopDto);
            return Ok(apiResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var apiResponse = await _sopService.GetAllSops();
            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetSopLatestVersion(int id)
        {
            var apiResponse = await _sopService.GetLatestSopVersion(id);
            return Ok(apiResponse);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateSop(int id, [FromBody] SopDto sopDto)
        {
            var apiResponse = await _sopService.UpdateSop(id, sopDto);
            return Ok(apiResponse);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteSop(int id)
        {
            var apiResponse = await _sopService.DeleteSop(id);
            return Ok(apiResponse);
        }
    }
}