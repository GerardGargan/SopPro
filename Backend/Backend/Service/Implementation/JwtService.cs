using Backend.Models.Settings;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend.Service.Implementation
{
    public class JwtService : IJwtService
    {
        private readonly ApplicationSettings _appSettings;
        public JwtService(IOptions<ApplicationSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public string GenerateToken(string email, string role)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_appSettings.JwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim("role", role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
            issuer: _appSettings.JwtIssuer,
            audience: _appSettings.JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_appSettings.JwtExpireHours),
            signingCredentials: credentials
        );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}
