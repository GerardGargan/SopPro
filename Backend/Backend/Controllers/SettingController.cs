using System.Net;
using Backend.Models;
using Backend.Models.Dto;
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
        [ProducesResponseType(200, Type = typeof(ApiResponse<SettingDto>))]
        public async Task<IActionResult> GetSettingByKey(string key)
        {
            SettingDto settingDto = await _settingService.GetSettingByKey(key);
            ApiResponse<SettingDto> apiResponse = new ApiResponse<SettingDto>()
            {
                IsSuccess = true,
                Result = settingDto,
                StatusCode = HttpStatusCode.OK,
            };

            return Ok(apiResponse);
        }
    }
}