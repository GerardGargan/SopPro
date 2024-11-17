namespace Backend.Repository.Interface
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository ApplicationUser { get; }
        public Task Save();
    }
}
