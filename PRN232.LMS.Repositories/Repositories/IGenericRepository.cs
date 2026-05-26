using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PRN232.LMS.Repositories.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetQueryable();
        Task<T?> GetByIdAsync(object id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveChangesAsync();
    }
}

