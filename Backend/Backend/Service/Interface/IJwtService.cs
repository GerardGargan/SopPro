using System.Security.Claims;

namespace Backend.Service.Interface
{
    public interface IJwtService
    {
        string GenerateToken(string email, string role, int organisationId, string issuer, string audience, int expiryHours, string secret);
        ClaimsPrincipal ValidateToken(string token, string secret, string issuer, string audiece);
    }
}
