using Backend.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/ppe")]
    [ApiController]
    [Authorize]

    public class PpeController : BaseApiController
    {
        private readonly IPpeService _ppeService;

        public PpeController(IPpeService ppeService)
        {
            _ppeService = ppeService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAll()
        {
            var apiResponse = await _ppeService.GetAll();
            return Ok(apiResponse);
        }
    }
}