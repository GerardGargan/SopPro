namespace Backend.Repository.Interface
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository ApplicationUser { get; }
        IOrganisationRepository Organisation { get; }
        public Task SaveAsync();
    }
}
