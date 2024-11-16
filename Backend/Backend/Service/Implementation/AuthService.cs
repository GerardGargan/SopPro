using Azure;
using Backend.Data;
using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Service.Interface;
using Backend.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace Backend.Service.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtService _jwtService;
        public string secretKey;

        public AuthService(ApplicationDbContext db, IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IJwtService jwtService)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApplicationSettings:JwtSecret");
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }
        public async Task<ApiResponse> RegisterUser(RegisterRequestDTO model, ModelStateDictionary modelState)
        {
            var apiResponse = new ApiResponse();
            var isError = false;
            // Check is email is already in use
            ApplicationUser userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == model.Email.ToLower());

            // User with that email exists
            if (userFromDb != null)
            {
                apiResponse.ErrorMessages.Add("Email is already in use");
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.IsSuccess = false;
                isError = true;
                return apiResponse;
            }

            if (!modelState.IsValid)
            {
                var errors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                apiResponse.ErrorMessages.AddRange(errors);
                isError = true;
            }

            if(isError)
            {
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.IsSuccess = false;
                return apiResponse;
            }

            ApplicationUser newUser = new ApplicationUser
            {
                Name = model.Name,
                UserName = model.Email,
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
            };


            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                // check if roles exists, create if not
                var doesAdminRoleExist = await _roleManager.RoleExistsAsync(StaticDetails.Role_Admin);
                var doesUserRoleExist = await _roleManager.RoleExistsAsync(StaticDetails.Role_User);

                if (!doesAdminRoleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin));
                }
                if (!doesUserRoleExist)
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

                return new ApiResponse<ApplicationUser>
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = newUser
                };
            } 
            else
            {
                throw new Exception("Failed to create user, unexpected error");
            }

        }

    }
}

