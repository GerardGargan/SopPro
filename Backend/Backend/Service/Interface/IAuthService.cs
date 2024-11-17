using Backend.Models;
using Backend.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Backend.Service.Interface
{
    public interface IAuthService
    {
        Task<ApiResponse> RegisterInvitedUser(RegisterInviteRequestDTO model, ModelStateDictionary modelState);
        Task<ApiResponse> InviteUser(InviteRequestDTO model, ModelStateDictionary modelState);
    }
}
