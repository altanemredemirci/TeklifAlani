using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TeklifAlani.Core.Interfaces;

namespace TeklifAlani.DAL.Concrete
{
    public class EfCoreGenericRepositoryDal<T, TContext> : IRepository<T>
         where T : class
         where TContext : DbContext, new()
    {

        public void Create(T entity)
        {
            using (var context = new TContext()) // DataContext db = new DataContext()
            {
                context.Set<T>().Add(entity); // db.Categories.Add(entity);
                context.SaveChanges();
            }
        }

        public void Delete(T entity)
        {
            using (var context = new TContext())
            {
                context.Set<T>().Remove(entity);
                context.SaveChanges();
            }
        }

        public List<T> GetAll(Expression<Func<T, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public virtual T GetById(int id)
        {
            using (var context = new TContext())
            {
                return context.Set<T>().Find(id);
            }
        }

        public virtual T GetOne(Expression<Func<T, bool>> filter)
        {
            using (var context = new TContext())
            {
                return filter == null
                    ? context.Set<T>().FirstOrDefault()
                    : context.Set<T>().Where(filter).FirstOrDefault();
            }
        }

        public virtual void Update(T entity)
        {
            using (var context = new TContext())
            {
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
