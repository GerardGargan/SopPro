using Backend.Data;
using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Service.Interface;
using Backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="ReguesterRequestDTO">A model representing a user and a token containing the users role and organisation</param>
        /// <returns>A confirmation of the created item.</returns>
        [HttpPost("registerinvite")]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> RegisterInvite([FromBody] RegisterInviteRequestDTO model)
        {
            var apiResponse = await _authService.RegisterInvitedUser(model, ModelState);

            if(apiResponse.IsSuccess)
            {
                return Ok(apiResponse);
            } else
            {
                return StatusCode((int)apiResponse.StatusCode, apiResponse);
            }
        }

        /// <summary>
        /// Invites a user to the system, encode email, role and organisation in a token.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>A success status</returns>

        [HttpPost("inviteuser")]
        //[Authorize(Roles = StaticDetails.Role_Admin)]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> InviteUser([FromBody] InviteRequestDTO model)
        {

            var apiResponse = await _authService.InviteUser(model, ModelState);

            if (apiResponse.IsSuccess)
            {
                return Ok(apiResponse);
            }
            else
            {
                return StatusCode((int)apiResponse.StatusCode, apiResponse);
            }

        }

    }
}
