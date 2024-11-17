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
            // Sanitise inputs
            model.Forename = model.Forename.Trim();
            model.Surname = model.Surname.Trim();

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
            if (model.Token == null)
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
            else if (invitationFromDb.Status == Status.Accepted)
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
            if (organisationFromDb == null)
            {
                apiResponse.ErrorMessages.Add("Organisation not found");
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.IsSuccess = false;
                return apiResponse;
            }

            ApplicationUser newUser = CreateApplicationUser(model.Forename, model.Surname, invitationFromDb.Email, invitationFromDb.OrganisationId);
            var result = await CreateUserWithRole(newUser, model.Password, invitationFromDb.Role);

            if (!result.Succeeded)
            {
                throw new Exception("Failed to create user, unexpected error");
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

        public async Task<ApiResponse> InviteUser(InviteRequestDTO model, ModelStateDictionary modelState)
        {
            // Sanitise inputs
            model.Email = model.Email.Trim().ToLower();
            model.Role = model.Role.Trim().ToLower();

            ApiResponse apiResponse = new ApiResponse();

            // Perform validation and error handling
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUsers.GetAsync(u => u.UserName.ToLower() == model.Email);

            var isError = false;

            if (userFromDb != null)
            {
                apiResponse.ErrorMessages.Add("User already exists");
                isError = true;
            }

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

            // TODO: Send the user an email with the token embedded in the link

            // return an api response with a success

            apiResponse.IsSuccess = true;
            apiResponse.StatusCode = HttpStatusCode.OK;
            apiResponse.SuccessMessage = "User invited successfully";

            return apiResponse;
        }

        public async Task<ApiResponse> SignupOrganisation(OrganisationSignupRequest model, ModelStateDictionary modelState)
        {
            // Sanitise inputs
            model.OrganisationName = model.OrganisationName.Trim();
            model.Email = model.Email.Trim().ToLower();
            model.Forename = model.Forename.Trim();
            model.Surname = model.Surname.Trim();

            ApiResponse apiResponse = new ApiResponse();
            bool isError = false;

            // Perform validation and error handling
            if (!modelState.IsValid)
            {
                var errors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                apiResponse.ErrorMessages.AddRange(errors);
                isError = true;
            }

            if (!ValidatePassword(model.Password))
            {
                apiResponse.ErrorMessages.Add("Password does not meet minimum requirements");
                isError = true;
            }

            // check is user already exists
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUsers.GetAsync(user => user.UserName.ToLower() == model.Email);

            if (userFromDb != null)
            {
                apiResponse.ErrorMessages.Add("User already exists");
                isError = true;
            }

            // check if organisation already exists

            Organisation organisationFromDb = await _unitOfWork.Organisations.GetAsync(organisation => organisation.Name.ToLower() == model.OrganisationName.Trim().ToLower());
            if (organisationFromDb != null)
            {
                apiResponse.ErrorMessages.Add("Organisation already exists");
                isError = true;
            }

            if (isError)
            {
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.IsSuccess = false;
                return apiResponse;
            }

            // Passed validation checks, create user and organisation

            Organisation newOrganisation = new Organisation
            {
                Name = model.OrganisationName,
            };

            await _unitOfWork.Organisations.AddAsync(newOrganisation);
            await _unitOfWork.SaveAsync();

            ApplicationUser newAdminUser = CreateApplicationUser(model.Forename, model.Surname, model.Email, newOrganisation.Id);
            
            var result = await CreateUserWithRole(newAdminUser, model.Password, StaticDetails.Role_Admin);
            if(!result.Succeeded)
            {
                throw new Exception("Failed to create user, unexpected error");
            }

            return new ApiResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                SuccessMessage = "Organisation and user created successfully"
            };
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

        private async Task EnsureRoleExists(string roleName)
        {
            if(!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private ApplicationUser CreateApplicationUser(string forename, string surname, string email, int organisationId)
        {
            return new ApplicationUser
            {
                Forename = forename,
                Surname = surname,
                UserName = email.ToLower(),
                Email = email.ToLower(),
                OrganisationId = organisationId,
                NormalizedEmail = email.ToUpper(),
            };
        }

        private async Task<IdentityResult> CreateUserWithRole(ApplicationUser user, string password, string role)
        {
            await EnsureRoleExists(role);
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            return result;
        }
    }
}

