using Azure;
using Backend.Data;
using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Models.Settings;
using Backend.Repository.Interface;
using Backend.Service.Interface;
using Backend.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

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
        private readonly ApplicationSettings _appSettings;
        public string secretKey;

        public AuthService(ApplicationDbContext db, IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IJwtService jwtService, IOptions<IdentityOptions> identityOptions, IUnitOfWork unitOfWork, IOptions<ApplicationSettings> appSettings)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApplicationSettings:JwtSecret");
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _identityOptions = identityOptions.Value;
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
        }
        public async Task<ApiResponse> RegisterInvitedUser(RegisterInviteRequestDTO model, ModelStateDictionary modelState)
        {
            var apiResponse = new ApiResponse();
            var isError = false;

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

            // Validate the token
            if(model.Token == null)
            {
                apiResponse.ErrorMessages.Add("Token is required");
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.IsSuccess = false;
                return apiResponse;
            }

            // Validate token exists in the invitations table
            Invitation invitationFromDb = await _unitOfWork.Invitations.GetAsync(invitation => invitation.Token == model.Token, tracked: true);
            
            if (invitationFromDb == null)
            {
                apiResponse.ErrorMessages.Add("Invitation not found");
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.IsSuccess = false;
                return apiResponse;
            } 
            else if (invitationFromDb.ExpiryDate < DateTime.UtcNow)
            {
                apiResponse.ErrorMessages.Add("Invitation has expired, please contact an administrator");
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.IsSuccess = false;
                return apiResponse;
            } 
            else if(invitationFromDb.Status == Status.Accepted)
            {
                apiResponse.ErrorMessages.Add("Invitation has already been accepted");
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.IsSuccess = false;
                return apiResponse;
            }

            ClaimsPrincipal claimsPrincipal = null;

            try
            {
                claimsPrincipal = _jwtService.ValidateToken(model.Token, _appSettings.JwtSecret, _appSettings.JwtIssuer, _appSettings.JwtAudience);
            }
            catch (Exception e)
            {
                apiResponse.ErrorMessages.Add("Error processing token");
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.IsSuccess = false;
                return apiResponse;
            }

            //var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(model.Token);

            //// Get token claims
            //var tokenEmail = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            //var tokenRole = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            //var organisationIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "organisationId")?.Value;

            //if (!int.TryParse(organisationIdClaim, out var tokenOrganisationId))
            //{
            //    apiResponse.ErrorMessages.Add("Error parsing organisation");
            //    apiResponse.StatusCode = HttpStatusCode.BadRequest;
            //    apiResponse.IsSuccess = false;
            //    return apiResponse;
            //}

            // Attempt to fetch a user to check if they already exist
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUsers.GetAsync(u => u.UserName.ToLower() == invitationFromDb.Email.ToLower());

            // User with that email exists
            if (userFromDb != null)
            {
                apiResponse.ErrorMessages.Add("Email is already in use");
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.IsSuccess = false;
                isError = true;
                return apiResponse;
            }

            // Validate that the organisation exists in the DB
            Organisation organisationFromDb = await _unitOfWork.Organisations.GetAsync(organisation => organisation.Id == invitationFromDb.OrganisationId);
            if (organisationFromDb == null) {
                apiResponse.ErrorMessages.Add("Organisation not found");
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.IsSuccess = false;
                return apiResponse;
            }

            ApplicationUser newUser = new ApplicationUser
            {
                Forename = model.Forename,
                Surname = model.Surname,
                UserName = invitationFromDb.Email.ToLower(),
                Email = invitationFromDb.Email.ToLower(),
                OrganisationId = invitationFromDb.OrganisationId,
                NormalizedEmail = invitationFromDb.Email.ToUpper(),
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
                if (invitationFromDb.Role.ToLower() == StaticDetails.Role_Admin)
                {
                    await _userManager.AddToRoleAsync(newUser, StaticDetails.Role_Admin);
                }
                else
                {
                    await _userManager.AddToRoleAsync(newUser, StaticDetails.Role_User);
                }

                // Update invitation to accepted
                invitationFromDb.Status = Status.Accepted;
                
                await _unitOfWork.SaveAsync();

                return new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    SuccessMessage = "User registration successful"
                };

            }
            else
            {
                throw new Exception("Failed to create user, unexpected error");
            }

        }

        public async Task<ApiResponse> InviteUser(InviteRequestDTO model, ModelStateDictionary modelState)
        {
            ApiResponse apiResponse = new ApiResponse();

            // Perform validation and error handling
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUsers.GetAsync(u => u.UserName.ToLower() == model.Email.ToLower());

            var isError = false;

            if (userFromDb != null)
            {
                apiResponse.ErrorMessages.Add("User already exists");
                isError = true;
            }
            /*
             * Need to validate role and organisation id
             * 
             */

            if (!modelState.IsValid)
            {
                var errors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                apiResponse.ErrorMessages.AddRange(errors);
                isError = true;
            }

            // If an error has occurred return a bad response with the ApiResponse object populated
            if (isError)
            {
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.IsSuccess = false;
                return apiResponse;
            }

            // Data is validated at this point, proceed to generate a token and store it in the database, send the user an invitation email
            string token = _jwtService.GenerateToken(model.Email, model.Role, model.OrganisationId, _appSettings.JwtIssuer, _appSettings.JwtAudience, _appSettings.JwtExpireHours, _appSettings.JwtSecret);

            // Store the token and relevant info in the database
            await _unitOfWork.Invitations.AddAsync(new Invitation
            {
                Email = model.Email,
                Role = model.Role,
                OrganisationId = model.OrganisationId,
                Token = token,
                Status = Status.Pending,
                ExpiryDate = DateTime.UtcNow.AddHours(_appSettings.JwtExpireHours),
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveAsync();

            // Send the user an email with the token embedded in the link

            // return an api response with a success

            apiResponse.IsSuccess = true;
            apiResponse.StatusCode = HttpStatusCode.OK;
            apiResponse.SuccessMessage = "User invited successfully";

            return apiResponse;
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

