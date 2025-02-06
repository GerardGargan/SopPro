using Backend.Models;
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
    }
}
