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
        private readonly ApplicationDbContext _db;
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtService _jwtService;
        public string secretKey;

        public AuthController(ApplicationDbContext db, IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IJwtService jwtService, IAuthService authService)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApplicationSettings:JwtSecret");
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _authService = authService;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="ReguesterRequestDTO">A model representing a user and a token containing the users role and organisation</param>
        /// <returns>A confirmation of the created item.</returns>
        [HttpPost("register")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<ApplicationUser>))]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            var apiResponse = await _authService.RegisterUser(model, ModelState);

            if(apiResponse.IsSuccess)
            {
                return Ok(apiResponse);
            } else
            {
                return BadRequest(apiResponse);
            }
        }


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
                return BadRequest(apiResponse);
            }

        }

        [HttpGet("test")]
        public string Test()
        {
            return _jwtService.GenerateToken("","");
        }
    }
}
