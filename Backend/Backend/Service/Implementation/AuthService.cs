using Azure;
using Backend.Data;
using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Repository.Interface;
using Backend.Service.Interface;
using Backend.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System.Net;

namespace Backend.Service.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly IdentityOptions _identityOptions;
        private readonly IUnitOfWork _unitOfWork;
        public string secretKey;

        public AuthService(ApplicationDbContext db, IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IJwtService jwtService, IOptions<IdentityOptions> identityOptions, IUnitOfWork unitOfWork)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApplicationSettings:JwtSecret");
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _identityOptions = identityOptions.Value;
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse> RegisterUser(RegisterRequestDTO model, ModelStateDictionary modelState)
        {
            var apiResponse = new ApiResponse();
            var isError = false;

            // Attempt to fetch a user to see if they already exist
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUser.GetAsync(u => u.UserName.ToLower() == model.Email.ToLower());

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

            if (!ValidatePassword(model.Password))
            {
                apiResponse.ErrorMessages.Add("Password does not meet requirements");
                isError = true;
            }

            if (isError)
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

        public bool ValidatePassword(string password)
        {
            // Check if the password length is sufficient
            if (password.Length < _identityOptions.Password.RequiredLength)
            {
                return false;
            }

            // Check for at least one digit
            if (_identityOptions.Password.RequireDigit && !password.Any(char.IsDigit))
            {
                return false;
            }

            // Check for at least one lowercase letter
            if (_identityOptions.Password.RequireLowercase && !password.Any(char.IsLower))
            {
                return false;
            }

            // Check for at least one uppercase letter
            if (_identityOptions.Password.RequireUppercase && !password.Any(char.IsUpper))
            {
                return false;
            }

            // Check for at least one non-alphanumeric character
            if (_identityOptions.Password.RequireNonAlphanumeric && password.All(char.IsLetterOrDigit))
            {
                return false;
            }

            // Check for unique characters
            if (_identityOptions.Password.RequiredUniqueChars > 0 && password.Distinct().Count() < _identityOptions.Password.RequiredUniqueChars)
            {
                return false;
            }

            return true;
        }

    }
}

