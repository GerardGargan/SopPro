namespace Backend.Repository.Interface
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository ApplicationUser { get; }
        IOrganisationRepository Organisation { get; }
        IInvitationRepository Invitation { get; }
        public Task SaveAsync();
    }
}
