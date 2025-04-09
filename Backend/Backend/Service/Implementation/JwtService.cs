using Backend.Data;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Models.Settings;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Service.Implementation
{
    public class JwtService : IJwtService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ApplicationSettings _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        public JwtService(ApplicationDbContext dbContext, IOptions<ApplicationSettings> appSettings, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _appSettings = appSettings.Value;
            _userManager = userManager;
        }

        /// <summary>
        /// Generates a JWT token and Refresh Token for the user to authenticate with
        /// </summary>
        /// <param name="userFromDb"></param>
        /// <param name="roles"></param>
        /// <returns>An AuthenticationResult object containing the token and refresh token</returns>
        public async Task<AuthenticationResult> GenerateAuthToken(ApplicationUser userFromDb, IList<string> roles)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = System.Text.Encoding.ASCII.GetBytes(_appSettings.JwtSecret);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userFromDb.UserName),
                    new Claim("forename", userFromDb.Forename),
                    new Claim("surname", userFromDb.Surname),
                    new Claim("id", userFromDb.Id.ToString()),
                    new Claim("organisationId", userFromDb.OrganisationId.ToString()),
                    new Claim(ClaimTypes.Email, userFromDb.UserName),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Audience = _appSettings.JwtAudience,
                Issuer = _appSettings.JwtIssuer,
                Expires = DateTime.UtcNow.AddDays(_appSettings.JwtAuthExpireDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);


            var jwtToken = tokenHandler.WriteToken(token);

            // Generate the refresh token
            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                UserId = userFromDb.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(_appSettings.JwtAuthRefreshEpireDays),
                Token = GenerateRefreshToken()
            };

            // Save refresh token in the database
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = jwtToken,
                RefreshToken = refreshToken.Token
            };

        }

        /// <summary>
        /// Refreshes a users JWT token when it has expired
        /// </summary>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("This token hasn't expired yet");
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefreshToken = await _dbContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                throw new UnauthorizedAccessException("This refresh token doesn't exist");
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                throw new UnauthorizedAccessException("This refresh token has expired");
            }

            if (storedRefreshToken.Invalidated)
            {
                throw new UnauthorizedAccessException("This refresh token has been invalidated");
            }

            if (storedRefreshToken.Used)
            {
                throw new UnauthorizedAccessException("This refresh token has been used");
            }

            if (storedRefreshToken.JwtId != jti)
            {
                throw new UnauthorizedAccessException("This refresh token doesn't match this JWT");
            }

            storedRefreshToken.Used = true;
            _dbContext.RefreshTokens.Update(storedRefreshToken);
            await _dbContext.SaveChangesAsync();

            var userId = validatedToken.Claims.Single(x => x.Type == "id").Value;

            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == userId); var roles = await _userManager.GetRolesAsync(user);
            return await GenerateAuthToken(user, roles);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.JwtSecret)),
                    ValidateIssuer = true,
                    ValidIssuer = _appSettings.JwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = _appSettings.JwtAudience,
                    ValidateLifetime = false
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Generates a refresh token
        /// </summary>
        /// <returns></returns>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// Generates a JWT used for inviting a user to join an organisation and sign up
        /// </summary>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <param name="organisationId"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="expiryHours"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Validaes that an invitation token is valid
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <param name="issuer"></param>
        /// <param name="audiece"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException"></exception>
        /// <exception cref="Exception"></exception>
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
