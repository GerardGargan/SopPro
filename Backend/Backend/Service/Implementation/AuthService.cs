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
using Microsoft.IdentityModel.Tokens;
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

        public AuthService(ApplicationDbContext db, IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IJwtService jwtService, IOptions<IdentityOptions> identityOptions, IUnitOfWork unitOfWork, IOptions<ApplicationSettings> appSettings)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _identityOptions = identityOptions.Value;
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
        }

        public async Task<ApiResponse<LoginResponseDTO>> Login(LoginRequestDTO model, ModelStateDictionary modelState)
        {
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUsers.GetAsync(user => user.UserName.ToLower() == model.Email.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(userFromDb, model.Password);

            if(userFromDb == null || !isValid)
            {
                throw new Exception("Email or password is incorrect");
            }

            var roles = await _userManager.GetRolesAsync(userFromDb);

            // Generate JWT Token
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = System.Text.Encoding.ASCII.GetBytes(_appSettings.JwtSecret);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("forename", userFromDb.Forename),
                    new Claim("surname", userFromDb.Surname),
                    new Claim("id", userFromDb.Id.ToString()),
                    new Claim(ClaimTypes.Email, userFromDb.UserName),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(_appSettings.JwtAuthExpireDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDTO loginResponse = new()
            {
                Email = userFromDb.Email,
                Token = tokenHandler.WriteToken(token),
            };

            if(loginResponse.Email == null || string.IsNullOrWhiteSpace(loginResponse.Token))
            {
                throw new Exception("Email or password is incorrect");
            }

            return new ApiResponse<LoginResponseDTO>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = loginResponse
            };

        }
        public async Task<ApiResponse> RegisterInvitedUser(RegisterInviteRequestDTO model, ModelStateDictionary modelState)
        {
            // Sanitise inputs
            model.Forename = model.Forename.Trim();
            model.Surname = model.Surname.Trim();

            if (!modelState.IsValid)
            {
                var errors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                throw new Exception(errors.FirstOrDefault());
            }

            if (!ValidatePassword(model.Password))
            {
                throw new Exception("Password does not meet requirements");
            }

            // Validate the token
            if (model.Token == null)
            {
                throw new Exception("Token is required");

            }

            // Validate token exists in the invitations table
            Invitation invitationFromDb = await _unitOfWork.Invitations.GetAsync(invitation => invitation.Token == model.Token, tracked: true);

            if (invitationFromDb == null)
            {
                throw new Exception("Invitation not found");
            }
            else if (invitationFromDb.ExpiryDate < DateTime.UtcNow)
            {
                throw new Exception("Invitation has expired, please contact an administrator");
            }
            else if (invitationFromDb.Status == Status.Accepted)
            {
                throw new Exception("Invitation has already been accepted");
            }

            ClaimsPrincipal claimsPrincipal = null;

            try
            {
                claimsPrincipal = _jwtService.ValidateInviteToken(model.Token, _appSettings.JwtSecret, _appSettings.JwtIssuer, _appSettings.JwtAudience);
            }
            catch (Exception e)
            {
                throw new Exception("Error processing token");
            }

            // Attempt to fetch a user to check if they already exist
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUsers.GetAsync(u => u.UserName.ToLower() == invitationFromDb.Email.ToLower());

            // User with that email exists
            if (userFromDb != null)
            {
                throw new Exception("Email is already in use");
            }

            Organisation organisationFromDb = await _unitOfWork.Organisations.GetAsync(organisation => organisation.Id == invitationFromDb.OrganisationId);
            if (organisationFromDb == null)
            {
                throw new Exception("Organisation not found");
            }

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                ApplicationUser newUser = CreateApplicationUser(model.Forename, model.Surname, invitationFromDb.Email, invitationFromDb.OrganisationId);
                var result = await CreateUserWithRole(newUser, model.Password, invitationFromDb.Role);

                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user, unexpected error");
                }

                // Update invitation to accepted
                invitationFromDb.Status = Status.Accepted;

            });
            
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

            // Perform validation and error handling
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUsers.GetAsync(u => u.UserName.ToLower() == model.Email);
            Organisation orgFromDb = await _unitOfWork.Organisations.GetAsync(o => o.Id == model.OrganisationId);

            if (userFromDb != null)
            {
                throw new Exception("User already exists");
            }

            if(orgFromDb == null)
            {
                throw new Exception("Organisation does not exist");
            }

            if (!modelState.IsValid)
            {
                var errors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                throw new Exception(errors.FirstOrDefault());
            }

            // Data is validated at this point, proceed to generate a token and store it in the database, send the user an invitation email
            string token = _jwtService.GenerateInviteToken(model.Email, model.Role, model.OrganisationId, _appSettings.JwtIssuer, _appSettings.JwtAudience, _appSettings.JwtInviteExpireHours, _appSettings.JwtSecret);

            // Store the token and relevant info in the database
            await _unitOfWork.Invitations.AddAsync(new Invitation
            {
                Email = model.Email,
                Role = model.Role,
                OrganisationId = model.OrganisationId,
                Token = token,
                Status = Status.Pending,
                ExpiryDate = DateTime.UtcNow.AddHours(_appSettings.JwtInviteExpireHours),
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveAsync();

            // TODO: Send the user an email with the token embedded in the link

            // return an api response with a success
            return new ApiResponse()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                SuccessMessage = "User invited successfully"
            };

        }

        public async Task<ApiResponse> SignupOrganisation(OrganisationSignupRequest model, ModelStateDictionary modelState)
        {
            // Sanitise inputs
            model.OrganisationName = model.OrganisationName.Trim();
            model.Email = model.Email.Trim().ToLower();
            model.Forename = model.Forename.Trim();
            model.Surname = model.Surname.Trim();

            // Perform validation and error handling
            if (!modelState.IsValid)
            {
                var errors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                throw new Exception(errors.FirstOrDefault());
            }

            if (!ValidatePassword(model.Password))
            {
                throw new Exception("Password does not meet minimum requirements");
            }

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var apiResponse = new ApiResponse();

                // check is user already exists
                ApplicationUser userFromDb = await _unitOfWork.ApplicationUsers.GetAsync(user => user.UserName.ToLower() == model.Email);

                if (userFromDb != null)
                {
                    throw new Exception("User already exists");
                }

                // check if organisation already exists

                Organisation organisationFromDb = await _unitOfWork.Organisations.GetAsync(organisation => organisation.Name.ToLower() == model.OrganisationName.Trim().ToLower());
                if (organisationFromDb != null)
                {
                    throw new Exception("Organisation already exists");
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

                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user, unexpected error");
                }
                await _unitOfWork.SaveAsync();


            });

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
            if (!await _roleManager.RoleExistsAsync(roleName))
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

