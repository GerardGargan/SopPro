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


        //[HttpPost("inviteuser")]
        ////[Authorize(Roles = StaticDetails.Role_Admin)]
        //public async Task<IActionResult> InviteUser([FromBody] InviteRequestDTO model)
        //{
            
        //    // Perform validation and error handling
        //    ApplicationUser userFromDb = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == model.Email.ToLower());
            
        //    var isError = false;

        //    if (userFromDb != null)
        //    {
        //        _response.ErrorMessages.Add("User already exists");
        //        isError = true;
        //    }
            
        //    if(!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        //        _response.ErrorMessages.AddRange(errors);
        //        isError = true;
        //    }

        //    // If an error has occurred return a bad response with the ApiResponse object populated
        //    if(isError)
        //    {
        //        _response.StatusCode = HttpStatusCode.BadRequest;
        //        _response.IsSuccess = false;
        //        return BadRequest(_response);
        //    }

        //    // Data is validated at this point, proceed to generate a token and store it in the database, send the user an invitation email
        //    string token = _jwtService.GenerateToken(model.Email, model.Role);

        //    // Store the token and relevant info in the database

        //    // Send the user an email with the token embedded in the link


        //    return Ok(new { test = "test" });
        //}

        [HttpGet("test")]
        public string Test()
        {
            return _jwtService.GenerateToken("","");
        }
    }
}
