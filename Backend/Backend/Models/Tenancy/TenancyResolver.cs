namespace Backend.Models.Tenancy
{
    public class TenancyResolver : ITenancyResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TenancyResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

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
    }
}