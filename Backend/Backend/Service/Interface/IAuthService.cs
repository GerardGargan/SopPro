using Backend.Models;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Backend.Service.Interface
{
    public interface IAuthService
    {
        Task<ApiResponse> RegisterInvitedUser(RegisterInviteRequestDTO model, ModelStateDictionary modelState);
        Task<ApiResponse> InviteUser(InviteRequestDTO model, ModelStateDictionary modelState);
        Task<ApiResponse> SignupOrganisation(OrganisationSignupRequest model, ModelStateDictionary modelState);
        Task<ApiResponse<LoginResponseDTO>> Login(LoginRequestDTO model, ModelStateDictionary modelState);
        Task<ApiResponse> ChangePassword(ChangePasswordDto model);
        Task ForgotPassword(ForgotPasswordRequest model);
        Task<ApiResponse> ResetPassword(ResetPasswordRequest model);
        Task<List<ApplicationUserDto>> GetAll();
        Task<ApplicationUserDto> GetById(string id);
        Task<List<RoleDto>> GetRoles();
        Task UpdateUser(ApplicationUserDto model);
        Task DeleteUser(string id);
        Task<AuthenticationResult> RefreshTokenAsync(RefreshTokenRequest model);
    }
}
