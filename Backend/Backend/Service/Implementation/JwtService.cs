using Backend.Data;
using Backend.Models.Settings;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend.Service.Implementation
{
    public class JwtService : IJwtService
    {
        private readonly ApplicationSettings _appSettings;
        public JwtService()
        {
        }
        public string GenerateInviteToken(string email, string role, int organisationId, string issuer, string audience, int expiryHours, string secret)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim("role", role),
            new Claim("organisationId", organisationId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiryHours),
            signingCredentials: credentials
        );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidateInviteToken(string token, string secret, string issuer, string audiece)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = issuer,
                ValidAudience = audiece,
                IssuerSigningKey = securityKey,
                ClockSkew = TimeSpan.Zero
            };

            try
            {

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Ensure the token is a JWT
                if (validatedToken is JwtSecurityToken jwtToken &&
                    jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return principal;
                }
            }
            catch (SecurityTokenException ex)
            {
                // Log exception if needed
                throw new SecurityTokenException("Invalid token.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while validating the token.", ex);
            }

            return null;
        }
    }
}
