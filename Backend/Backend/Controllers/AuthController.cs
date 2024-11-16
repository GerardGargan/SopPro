using Backend.Data;
using Backend.Models;
using Backend.Models.Dto;
using Backend.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public string secretKey;

        public AuthController(ApplicationDbContext db, IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _response = new ApiResponse();
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _userManager = userManager;
            _roleManager = roleManager;
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
    }
}
