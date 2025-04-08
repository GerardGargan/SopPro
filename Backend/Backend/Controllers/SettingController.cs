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

        /// <summary>
        /// Gets a setting by its key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a setting
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Create([FromBody] SettingDto model)
        {
            await _settingService.Create(model);
            ApiResponse apiResponse = new ApiResponse()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                SuccessMessage = "Setting Created"
            };

            return Ok(apiResponse);
        }

        /// <summary>
        /// Deletes a setting by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:int}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Delete(int id)
        {
            await _settingService.Delete(id);
            ApiResponse apiResponse = new ApiResponse()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                SuccessMessage = "Setting deleted"
            };

            return Ok(apiResponse);
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{key}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Update([FromBody] SettingDto model)
        {
            await _settingService.Update(model);
            ApiResponse apiResponse = new ApiResponse()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                SuccessMessage = "Setting updated"
            };

            return Ok(apiResponse);
        }
    }

}