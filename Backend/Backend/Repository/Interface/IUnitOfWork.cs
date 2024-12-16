using Microsoft.EntityFrameworkCore.Storage;

namespace Backend.Repository.Interface
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository ApplicationUsers { get; }
        IOrganisationRepository Organisations { get; }
        IInvitationRepository Invitations { get; }
        IDepartmentRepository Departments { get; }
        public Task SaveAsync();
        public Task ExecuteInTransactionAsync(Func<Task> action);
        public Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);

    }
}
