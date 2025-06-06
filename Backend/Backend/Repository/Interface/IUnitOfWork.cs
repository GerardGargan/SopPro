﻿
namespace Backend.Repository.Interface
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository ApplicationUsers { get; }
        IOrganisationRepository Organisations { get; }
        IInvitationRepository Invitations { get; }
        IDepartmentRepository Departments { get; }
        IPpeRepository Ppe { get; }
        ISopRepository Sops { get; }
        ISopHazardRepository SopHazards { get; }
        ISopStepRepository SopSteps { get; }
        ISopVersionRepository SopVersions { get; }
        ISopUserFavouriteRepository SopUserFavourites { get; }
        ISettingRepository Settings { get; }
        public Task SaveAsync();
        public Task ExecuteInTransactionAsync(Func<Task> action);
        public Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);

    }
}
