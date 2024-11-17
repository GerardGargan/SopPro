namespace Backend.Repository.Interface
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository ApplicationUsers { get; }
        IOrganisationRepository Organisations { get; }
        IInvitationRepository Invitations { get; }
        public Task SaveAsync();
    }
}
