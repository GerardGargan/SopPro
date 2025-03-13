using Backend.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/setting")]
    [ApiController]
    [Authorize]
    public class SettingController : BaseApiController
    {
        private readonly ISettingService _settingService;

        public SettingController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpGet]
        [Route("{key}")]
        public async Task<IActionResult> GetSettingByKey(string key)
        {
            return Ok("Test");
        }
    }
}