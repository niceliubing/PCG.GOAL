using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccess.Common.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(int id);

        IQueryable<T> Find(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties);
        T Single(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties);
        T First(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties);
        Task<T> FirstAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties);
    }
}
