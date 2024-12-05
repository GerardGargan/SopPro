using System.Linq.Expressions;

namespace Backend.Repository.Interface
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null, string includeProperties = null, bool tracked = false);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, string includeProperties = null, bool tracked = false);
        Task AddAsync(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
