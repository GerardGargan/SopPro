using System.Net;
using Backend.Models;
using Backend.Models.Dto;
using Backend.Service.Interface;
using Backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<LoginResponseDTO>))]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var apiResponse = await _authService.Login(model, ModelState);

            return StatusCode((int)apiResponse.StatusCode, apiResponse);
        }

        [HttpPost("signuporganisation")]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SignupOrganisation([FromBody] OrganisationSignupRequest model)
        {
            var apiResponse = await _authService.SignupOrganisation(model, ModelState);

            if (apiResponse.IsSuccess)
            {
                return Ok(apiResponse);
            }
            else
            {
                return StatusCode((int)apiResponse.StatusCode, apiResponse);
            }
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

            if (apiResponse.IsSuccess)
            {
                return Ok(apiResponse);
            }
            else
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
        [Authorize(Roles = StaticDetails.Role_Admin)]
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

        [HttpGet]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAll()
        {
            List<ApplicationUserDto> allUsers = await _authService.GetAll();

            ApiResponse<List<ApplicationUserDto>> apiResponse = new ApiResponse<List<ApplicationUserDto>>()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = allUsers
            };

            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetUser([FromRoute] string id)
        {
            ApplicationUserDto userDto = await _authService.GetById(id);

            var apiResponse = new ApiResponse<ApplicationUserDto>()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = userDto
            };

            return Ok(apiResponse);
        }


        [HttpGet("roles")]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetRoles()
        {
            var allRoles = await _authService.GetRoles();

            ApiResponse<List<RoleDto>> apiResponse = new ApiResponse<List<RoleDto>>()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = allRoles
            };

            return Ok(apiResponse);
        }

        [Authorize]
        [HttpPost("password")]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var apiResponse = await _authService.ChangePassword(model);
            return StatusCode((int)apiResponse.StatusCode, apiResponse);
        }

        [AllowAnonymous]
        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            await _authService.ForgotPassword(model);

            return Ok("Email sent with a reset link");
        }

        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        [HttpPost("reset")]
        public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordRequest model)
        {
            var apiResponse = await _authService.ResetPassword(model);

            return Ok(apiResponse);
        }

        [AllowAnonymous]
        [HttpGet("redirect")]
        public async Task<IActionResult> RedirectToCustomScheme([FromQuery] string redirect)
        {
            if (string.IsNullOrWhiteSpace(redirect))
            {
                return BadRequest("Invalid redirect URL.");
            }

            if (!redirect.Contains("soppro://"))
            {
                return BadRequest("Invalid redirect scheme.");
            }

            return Redirect(redirect);
        }
    }
}
