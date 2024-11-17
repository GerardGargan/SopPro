using Microsoft.EntityFrameworkCore.Storage;

namespace Backend.Repository.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IApplicationUserRepository ApplicationUsers { get; }
        IOrganisationRepository Organisations { get; }
        IInvitationRepository Invitations { get; }
        public Task SaveAsync();
        public Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
