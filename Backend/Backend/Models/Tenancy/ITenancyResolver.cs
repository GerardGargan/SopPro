namespace Backend.Models.Tenancy
{
    public interface ITenancyResolver
    {
        public int? GetOrganisationid();
        public string GetUserId();
    }
}