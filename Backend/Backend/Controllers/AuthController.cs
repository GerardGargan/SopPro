using System.Net;
using Backend.Models;
using Backend.Models.DatabaseModels;
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

        /// <summary>
        /// Log in for users
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<LoginResponseDTO>))]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var apiResponse = await _authService.Login(model, ModelState);

            return StatusCode((int)apiResponse.StatusCode, apiResponse);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthenticationResult
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(error => error.ErrorMessage))
                });
            }

            var result = await _authService.RefreshTokenAsync(model);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates an organisation and administrator of that organisation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets a user by their id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(ApiResponse<ApplicationUserDto>))]
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

        /// <summary>
        /// Updates a user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = StaticDetails.Role_Admin)]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] ApplicationUserDto model)
        {
            await _authService.UpdateUser(model);

            var apiResponse = new ApiResponse()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                SuccessMessage = "User updated"
            };

            return Ok(apiResponse);
        }

        /// <summary>
        /// Gets a list of user roles
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = StaticDetails.Role_Admin)]
        [Route("{id}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            await _authService.DeleteUser(id);

            var apiResponse = new ApiResponse()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                SuccessMessage = "User deleted"
            };

            return Ok(apiResponse);
        }

        /// <summary>
        /// Change password request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("password")]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var apiResponse = await _authService.ChangePassword(model);
            return StatusCode((int)apiResponse.StatusCode, apiResponse);
        }

        /// <summary>
        /// Forgot password request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            await _authService.ForgotPassword(model);

            return Ok("Email sent with a reset link");
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        [HttpPost("reset")]
        public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordRequest model)
        {
            var apiResponse = await _authService.ResetPassword(model);

            return Ok(apiResponse);
        }

        /// <summary>
        /// Redirect to a url
        /// </summary>
        /// <param name="redirect"></param>
        /// <returns></returns>
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
