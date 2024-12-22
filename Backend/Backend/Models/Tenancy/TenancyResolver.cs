namespace Backend.Models.Tenancy
{
    public class TenancyResolver : ITenancyResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TenancyResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetOrganisationid()
        {
            if(_httpContextAccessor == null || _httpContextAccessor.HttpContext == null)
            {
                return -1;
            }

            int organisationId = int.TryParse(_httpContextAccessor.HttpContext.User?.FindFirst("organisationId")?.Value, out organisationId) ? organisationId : -1;

            if(organisationId == -1)
            {
                throw new UnauthorizedAccessException("OrganisationId is missing from the token");
            }
            return organisationId;
        }
    }
}