using System.Security.Claims;

namespace Backend.Models.Tenancy
{
    /// <summary>
    /// Helper class used to obtain a users organisationId, or userId based on JWT claims
    /// </summary>
    public class TenancyResolver : ITenancyResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TenancyResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Returns the organisation id from the users JWT claim
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public int? GetOrganisationid()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return null; // Handle unauthenticated requests
            }

            var organisationIdClaim = user.Claims.FirstOrDefault(c => c.Type == "organisationId")?.Value;

            if (int.TryParse(organisationIdClaim, out var organisationId))
            {
                return organisationId;
            }

            throw new InvalidOperationException("Organisation ID is missing or invalid in the token.");
        }

        /// <summary>
        /// Returns the users id
        /// </summary>
        /// <returns></returns>
        public string GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
            return userId;
        }
    }
}