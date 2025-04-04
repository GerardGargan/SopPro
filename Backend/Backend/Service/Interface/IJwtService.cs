using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using System.Security.Claims;

namespace Backend.Service.Interface
{
    public interface IJwtService
    {
        Task<AuthenticationResult> GenerateAuthToken(ApplicationUser userFromDb, IList<string> roles);
        string GenerateInviteToken(string email, string role, int organisationId, string issuer, string audience, int expiryHours, string secret);
        ClaimsPrincipal ValidateInviteToken(string token, string secret, string issuer, string audiece);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
    }
}
