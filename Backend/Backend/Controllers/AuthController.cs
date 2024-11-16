using Backend.Data;
using Backend.Models;
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
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApiResponse _response;
        private readonly IJwtService _jwtService;
        public string secretKey;

        public AuthController(ApplicationDbContext db, IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IJwtService jwtService)
        {
            _db = db;
            _response = new ApiResponse();
            secretKey = configuration.GetValue<string>("ApplicationSettings:JwtSecret");
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            // Check is email is already in use
            ApplicationUser userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == model.Email.ToLower());
            
            // User with that email exists
            if(userFromDb != null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("User already exists");
                return BadRequest(_response);
            }

            ApplicationUser newUser = new ApplicationUser
            {
                Name = model.Name,
                UserName = model.Email,
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
            };

            try
            {
                var result = await _userManager.CreateAsync(newUser, model.Password);

                if(result.Succeeded)
                {
                    // check if roles exists, create if not
                    var doesAdminRoleExist = await _roleManager.RoleExistsAsync(StaticDetails.Role_Admin);
                    var doesUserRoleExist = await _roleManager.RoleExistsAsync(StaticDetails.Role_User);

                    if(!doesAdminRoleExist)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin));
                    }
                    if(!doesUserRoleExist)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_User));
                    }

                    // assign user role, default to user if not admin
                    if (model.Role == StaticDetails.Role_Admin)
                    {
                        await _userManager.AddToRoleAsync(newUser, StaticDetails.Role_Admin);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(newUser, StaticDetails.Role_User);
                    }

                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    return Ok(_response);
                }

            }
            catch(Exception ex)
            {
            }

            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Error while registering user");

            return BadRequest(_response);
        }


        [HttpPost("inviteuser")]
        //[Authorize(Roles = StaticDetails.Role_Admin)]
        public async Task<IActionResult> InviteUser([FromBody] InviteRequestDTO model)
        {
            // Perform validation and error handling
            ApplicationUser userFromDb = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == model.Email.ToLower());
            
            var isError = false;

            if (userFromDb != null)
            {
                _response.ErrorMessages.Add("User already exists");
                isError = true;
            }
            
            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _response.ErrorMessages.AddRange(errors);
                isError = true;
            }

            // If an error has occurred return a bad response with the ApiResponse object populated
            if(isError)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            // Data is validated at this point, proceed to generate a token and store it in the database, send the user an invitation email
            


            return Ok(new { test = "test" });
        }

        [HttpGet("test")]
        public string Test()
        {
            return _jwtService.GenerateToken("","");
        }
    }
}
