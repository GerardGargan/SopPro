using Backend.Models.DatabaseModels;
using System.Security.Claims;

namespace Backend.Service.Interface
{
    public interface IJwtService
    {
        string GenerateAuthToken(ApplicationUser userFromDb, IList<string> roles, string jwtSecret, int jwtAuthExpireDays);
        string GenerateInviteToken(string email, string role, int organisationId, string issuer, string audience, int expiryHours, string secret);
        ClaimsPrincipal ValidateInviteToken(string token, string secret, string issuer, string audiece);
    }
}
