
using System.Linq.Expressions;

namespace TeklifAlani.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter);
        T GetOne(Expression<Func<T, bool>> filter);
        T GetById(int id);

        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
