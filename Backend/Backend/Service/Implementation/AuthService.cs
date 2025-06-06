﻿using Backend.Data;
using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Models.Settings;
using Backend.Models.Tenancy;
using Backend.Repository.Interface;
using Backend.Service.Interface;
using Backend.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;

namespace Backend.Service.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly IdentityOptions _identityOptions;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationSettings _appSettings;
        private readonly ITenancyResolver _tenancyResolver;
        private readonly IEmailService _emailService;
        private readonly ITemplateService _templateService;
        private readonly ISopService _sopService;

        public AuthService(ApplicationDbContext db, IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IJwtService jwtService, IOptions<IdentityOptions> identityOptions, IUnitOfWork unitOfWork, IOptions<ApplicationSettings> appSettings, ITenancyResolver tenancyResolver, IEmailService emailService, ITemplateService templateService, SignInManager<ApplicationUser> signInManager, ISopService sopService)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _identityOptions = identityOptions.Value;
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
            _tenancyResolver = tenancyResolver;
            _emailService = emailService;
            _templateService = templateService;
            _sopService = sopService;
        }

        /// <summary>
        /// Attempts to log in a user with the provided credentials.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse<LoginResponseDTO>> Login(LoginRequestDTO model, ModelStateDictionary modelState)
        {
            // Check if the user exists
            ApplicationUser userFromDb = await _db.ApplicationUsers.IgnoreQueryFilters().Where(x => x.Email == model.Email.ToLower()).FirstOrDefaultAsync();

            if (userFromDb == null)
            {
                throw new ArgumentException("Email or password is incorrect");
            }

            // Check if user is locked out

            if (await _userManager.IsLockedOutAsync(userFromDb))
            {
                return new ApiResponse<LoginResponseDTO>
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    IsSuccess = false,
                    ErrorMessage = "Your account is locked due to multiple failed attempts. Try again later."
                };
            }

            // Attempt to sign in the user with their password

            var result = await _signInManager.PasswordSignInAsync(userFromDb, model.Password, false, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return new ApiResponse<LoginResponseDTO>
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    IsSuccess = false,
                    ErrorMessage = "Your account is locked due to multiple failed attempts. Try again later."
                };
            }

            if (!result.Succeeded)
            {
                return new ApiResponse<LoginResponseDTO>
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    IsSuccess = false,
                    ErrorMessage = "Email or password is incorrect"
                };
            }

            // Reset failed attempts after successful login
            await _userManager.ResetAccessFailedCountAsync(userFromDb);

            var roles = await _userManager.GetRolesAsync(userFromDb);

            // Generate JWT Token
            AuthenticationResult authResult = await _jwtService.GenerateAuthToken(userFromDb, roles);

            LoginResponseDTO loginResponse = new()
            {
                Email = userFromDb.Email,
                Forename = userFromDb.Forename,
                Surname = userFromDb.Surname,
                Role = roles.FirstOrDefault(),
                Token = authResult.Token,
                RefreshToken = authResult.RefreshToken
            };

            if (loginResponse.Email == null || string.IsNullOrWhiteSpace(loginResponse.Token))
            {
                throw new ArgumentException("Email or password is incorrect");
            }

            return new ApiResponse<LoginResponseDTO>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = loginResponse
            };

        }

        /// <summary>
        /// Refreshes a users token on expiry of their JWT
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> RefreshTokenAsync(RefreshTokenRequest model)
        {
            var result = await _jwtService.RefreshTokenAsync(model.Token, model.RefreshToken);
            return result;
        }

        /// <summary>
        /// Retrieves all users
        /// </summary>
        /// <returns></returns>
        public async Task<List<ApplicationUserDto>> GetAll()
        {
            // Get a dictionary of user roles
            Dictionary<string, string> UserRoleLookup = await _db.UserRoles.ToDictionaryAsync(x => x.UserId, x => x.RoleId);

            List<ApplicationUserDto> allUsers = await _unitOfWork.ApplicationUsers
                .GetAll()
                .Select(x => new ApplicationUserDto()
                {
                    Id = x.Id,
                    Forename = x.Forename,
                    Surname = x.Surname,
                    OrganisationId = x.OrganisationId,
                    RoleId = UserRoleLookup.GetValueOrDefault(x.Id, null),
                    Email = x.Email
                }).ToListAsync();

            return allUsers;

        }

        /// <summary>
        /// Retrieves a single user by their id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApplicationUserDto> GetById(string id)
        {
            // Validate model
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id cant be empty");
            }

            var userFromDb = await _unitOfWork.ApplicationUsers.GetAsync(x => x.Id == id);
            if (userFromDb == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Get the users role
            string userRoleId = await _db.UserRoles.Where(x => x.UserId == userFromDb.Id).Select(x => x.RoleId).FirstOrDefaultAsync();
            string roleName = await _db.Roles.Where(x => x.Id == userRoleId).Select(x => x.Name).FirstOrDefaultAsync();

            ApplicationUserDto userDto = new ApplicationUserDto()
            {
                Forename = userFromDb.Forename,
                Surname = userFromDb.Surname,
                Email = userFromDb.Email,
                Id = userFromDb.Id,
                RoleId = userRoleId,
                RoleName = roleName
            };

            return userDto;
        }

        /// <summary>
        /// Updates a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UpdateUser(ApplicationUserDto model)
        {
            // Validate model

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                throw new ArgumentException("Id cant be null");
            }

            if (string.IsNullOrEmpty(model.Forename))
            {
                throw new ArgumentException("Forename cant be empty");
            }

            if (string.IsNullOrEmpty(model.Surname))
            {
                throw new ArgumentException("Surname cant be empty");
            }

            if (string.IsNullOrEmpty(model.RoleName))
            {
                throw new ArgumentException("Role cant be empty");
            }

            if (model.RoleName != StaticDetails.Role_Admin && model.RoleName != StaticDetails.Role_User)
            {
                throw new ArgumentException("Invalid role provided");
            }

            // Fetch the user from the database
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUsers.GetAsync(x => x.Id == model.Id, tracked: true);
            var currentRoles = await _userManager.GetRolesAsync(userFromDb);

            if (userFromDb == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Perform updates inside a transaction, if one execution fails all changes will be discarded
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {

                // Safe to continue updating
                userFromDb.Forename = model.Forename;
                userFromDb.Surname = model.Surname;

                // Remove any current roles before adding the new role
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(userFromDb, currentRoles);
                }

                await _userManager.AddToRoleAsync(userFromDb, model.RoleName.ToLower());

                await _unitOfWork.SaveAsync();
            });
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeleteUser(string id)
        {
            // Fetch the user
            ApplicationUser userFromDb = await _userManager.FindByIdAsync(id);

            // Validate the model
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("User id cant be null");
            }

            if (userFromDb == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Dont allow users to delete themselves, could leave the application with no users or admins in a tenancy/organisation
            if (id == _tenancyResolver.GetUserId())
            {
                throw new ArgumentException("You can't delete your own account while logged in");
            }

            // Execute in a transacton, so that all changes either persist or none if something fails
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Delete User Sop Favourites
                await _sopService.RemoveAllUserFavourites(id, false);

                // Set Author and ApprovedBy to null on sopVersions for this user (FK References)
                await _unitOfWork.SopVersions.GetAll(x => x.AuthorId == id).ExecuteUpdateAsync(x => x.SetProperty(x => x.AuthorId, (string)null));
                await _unitOfWork.SopVersions.GetAll(x => x.ApprovedById == id).ExecuteUpdateAsync(x => x.SetProperty(x => x.ApprovedById, (string)null));

                // Delete the user
                await _userManager.DeleteAsync(userFromDb);
                await _unitOfWork.SaveAsync();
            });
        }

        /// <summary>
        /// Registers a user with a valid invitation and token
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
                throw new ArgumentException("Password does not meet requirements");
            }

            // Validate the token
            if (model.Token == null)
            {
                throw new ArgumentException("Token is required");

            }

            // Validate token exists in the invitations table
            Invitation invitationFromDb = await _unitOfWork.Invitations.GetAsync(invitation => invitation.Token == model.Token, tracked: true);

            if (invitationFromDb == null)
            {
                throw new KeyNotFoundException("Invitation not found");
            }
            else if (invitationFromDb.ExpiryDate < DateTime.UtcNow)
            {
                throw new ArgumentException("Invitation has expired, please contact an administrator");
            }
            else if (invitationFromDb.Status == Status.Accepted)
            {
                throw new ArgumentException("Invitation has already been accepted");
            }

            ClaimsPrincipal claimsPrincipal = null;

            try
            {
                claimsPrincipal = _jwtService.ValidateInviteToken(model.Token, _appSettings.JwtSecret, _appSettings.JwtIssuer, _appSettings.JwtAudience);
            }
            catch (Exception)
            {
                throw new Exception("Error processing token");
            }

            // Attempt to fetch a user to check if they already exist
            ApplicationUser userFromDb = await _db.ApplicationUsers.IgnoreQueryFilters().Where(x => x.UserName.ToLower() == invitationFromDb.Email.ToLower()).FirstOrDefaultAsync();

            // User with that email exists
            if (userFromDb != null)
            {
                throw new ArgumentException("Email is already in use");
            }

            Organisation organisationFromDb = await _unitOfWork.Organisations.GetAsync(organisation => organisation.Id == invitationFromDb.OrganisationId);
            if (organisationFromDb == null)
            {
                throw new KeyNotFoundException("Organisation not found");
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
                await _unitOfWork.SaveAsync();

            });

            return new ApiResponse
            {
                StatusCode = HttpStatusCode.Created,
                IsSuccess = true,
                SuccessMessage = "User registration successful"
            };
        }

        /// <summary>
        /// Invites a user to an organisation with a valid jwt token
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse> InviteUser(InviteRequestDTO model, ModelStateDictionary modelState)
        {
            // Sanitise inputs
            model.Email = model.Email.Trim().ToLower();
            model.Role = model.Role.Trim().ToLower();

            // Perform validation and error handling
            ApplicationUser userFromDb = await _db.ApplicationUsers.IgnoreQueryFilters().Where(x => x.UserName.ToLower() == model.Email).FirstOrDefaultAsync();

            var orgId = _tenancyResolver.GetOrganisationid();
            Organisation orgFromDb = await _unitOfWork.Organisations.GetAsync(o => o.Id == orgId);

            if (userFromDb != null)
            {
                throw new ArgumentException("User already exists");
            }

            if (orgFromDb == null)
            {
                throw new KeyNotFoundException("Organisation does not exist");
            }

            if (model.Role != StaticDetails.Role_Admin && model.Role != StaticDetails.Role_User)
            {
                throw new ArgumentException("Invalid role selected");
            }

            if (!modelState.IsValid)
            {
                var errors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                throw new ArgumentException(errors.FirstOrDefault());
            }

            // Data is validated at this point, proceed to generate a token and store it in the database, send the user an invitation email
            string token = _jwtService.GenerateInviteToken(model.Email, model.Role, orgFromDb.Id, _appSettings.JwtIssuer, _appSettings.JwtAudience, _appSettings.JwtInviteExpireHours, _appSettings.JwtSecret);

            // Store the token and relevant info in the database
            await _unitOfWork.Invitations.AddAsync(new Invitation
            {
                Email = model.Email,
                Role = model.Role,
                OrganisationId = orgFromDb.Id,
                Token = token,
                Status = Status.Pending,
                ExpiryDate = DateTime.UtcNow.AddHours(_appSettings.JwtInviteExpireHours),
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveAsync();

            // Send the user an email with the token embedded in the link
            var encodedToken = Uri.EscapeDataString(token);

            var deepLinkUrl = $"soppro://registerinvite?token={encodedToken}";
            var redirectUrl = $"{_appSettings.BaseUrl}/api/auth/redirect?redirect={Uri.EscapeDataString(deepLinkUrl)}";

            var invitedByUser = await _unitOfWork.ApplicationUsers.GetAsync(x => x.Id == _tenancyResolver.GetUserId());

            var emailModel = new
            {
                OrganisatonName = orgFromDb.Name,
                InvitedBy = invitedByUser.Forename,
                DeepLink = redirectUrl
            };

            string emailSubject = $"{emailModel.InvitedBy} has invited you to join {orgFromDb.Name} on SopPro";
            string emailBody = await _templateService.RenderTemplateAsync("UserInvitation", emailModel);

            _ = _emailService.SendEmailAsync(model.Email, emailSubject, emailBody);

            // return an api response with a success
            return new ApiResponse()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                SuccessMessage = "User invited successfully"
            };

        }

        /// <summary>
        /// Creates an organisation and admin user associated with that organisation
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse> SignupOrganisation(OrganisationSignupRequest model, ModelStateDictionary modelState)
        {
            // Sanitise inputs
            model.OrganisationName = model.OrganisationName?.Trim();
            model.Email = model.Email?.Trim().ToLower();
            model.Forename = model.Forename?.Trim();
            model.Surname = model.Surname?.Trim();

            // Perform validation and error handling
            if (!modelState.IsValid)
            {
                var errors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                throw new ArgumentException(errors.FirstOrDefault());
            }

            if (!ValidatePassword(model.Password))
            {
                throw new ArgumentException("Password does not meet minimum requirements");
            }

            // Execute database queries in a transaction so it rolls back if one fails
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var apiResponse = new ApiResponse();

                // check is user already exists
                ApplicationUser userFromDb = await _db.ApplicationUsers.IgnoreQueryFilters().Where(x => x.UserName.ToLower() == model.Email).FirstOrDefaultAsync();

                if (userFromDb != null)
                {
                    throw new ArgumentException("User already exists");
                }

                // check if organisation already exists

                Organisation organisationFromDb = await _unitOfWork.Organisations.GetAsync(organisation => organisation.Name.ToLower() == model.OrganisationName.Trim().ToLower());
                if (organisationFromDb != null)
                {
                    throw new ArgumentException("Organisation already exists");
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
                StatusCode = HttpStatusCode.Created,
                IsSuccess = true,
                SuccessMessage = "Organisation and user created successfully"
            };
        }

        /// <summary>
        /// Changes a users password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse> ChangePassword(ChangePasswordDto model)
        {
            // Validate password meets minimum requirements
            if (!ValidatePassword(model.NewPassword))
            {
                throw new ArgumentException("Password does not meet the minimum criteria");
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                throw new ArgumentException("New password and confirm password do not match");
            }

            // Fetch the user by their id
            var userId = _tenancyResolver.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);

            // check if the existing password is correct for the user
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!isPasswordCorrect)
            {
                throw new ArgumentException("Old Passowrd is incorrect");
            }

            // Change the users password
            var changePassword = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePassword.Succeeded)
            {
                throw new Exception("Oops something went wrong!");
            }

            return new ApiResponse()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                SuccessMessage = "Password changed successfully"
            };
        }

        /// <summary>
        /// Initiates the password reset process by generating a reset token and sending a reset link to the user's email.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task ForgotPassword(ForgotPasswordRequest model)
        {
            // Fetch user
            var user = await _db.ApplicationUsers.IgnoreQueryFilters().Where(x => x.UserName == model.Email.ToLower()).FirstOrDefaultAsync();

            // If the user exists, send a reset token, otherwise dont send anything
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = Uri.EscapeDataString(token);

                var resetUrl = $"soppro://reset?email={model.Email}&token={encodedToken}";
                var redirectUrl = $"{_appSettings.BaseUrl}/api/auth/redirect?redirect={Uri.EscapeDataString(resetUrl)}";

                _ = _emailService.SendEmailAsync(model.Email, "Password Reset", $@"<a href=""{redirectUrl}"">Click here to reset your password</a>");
            }
        }

        /// <summary>
        /// Resets a users password with a valid reset token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse> ResetPassword(ResetPasswordRequest model)
        {
            if (!ValidatePassword(model.NewPassword))
            {
                throw new ArgumentException("Password does not meet requirements");
            }

            var user = await _db.ApplicationUsers.IgnoreQueryFilters().Where(x => x.UserName == model.Email.ToLower()).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new KeyNotFoundException("User could not be found");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.ResetCode, model.NewPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Error reseting password");
            }

            return new ApiResponse()
            {
                IsSuccess = true,
                SuccessMessage = "Password reset successfully",
                StatusCode = HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Retrieves a list of user roles
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoleDto>> GetRoles()
        {
            List<RoleDto> allRoles = await _db.Roles.Select(x => new RoleDto()
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            return allRoles;
        }

        /// <summary>
        /// Function that validates if a password meets minimum requirements
        /// </summary>
        /// <param name="password"></param>
        /// <returns>True if valid, false if invalid</returns>
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

        /// <summary>
        /// Checks if a role exists, if it does not exist is creates it
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        private async Task EnsureRoleExists(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        /// <summary>
        /// Creates an application user
        /// </summary>
        /// <param name="forename"></param>
        /// <param name="surname"></param>
        /// <param name="email"></param>
        /// <param name="organisationId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a user and assigns them a specified role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
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

