using System.Security.Claims;

namespace Backend.Service.Interface
{
    public interface IJwtService
    {
        string GenerateToken(string email, string role);
        ClaimsPrincipal ValidateToken(string token);
    }
}
