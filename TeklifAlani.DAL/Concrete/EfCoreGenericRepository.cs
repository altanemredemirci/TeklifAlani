﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
    public class EfCoreGenericRepository<T, TContext> : IRepository<T>
        where T : class
        where TContext : DbContext
    {

        protected readonly TContext _context;

        public EfCoreGenericRepository(TContext context)
        {
            _context = context;
        }


        public void Create(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public virtual T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public virtual T GetOne(Expression<Func<T, bool>> filter)
        {            
                return _context.Set<T>().FirstOrDefault(filter);            
        }

        public virtual void Update(T entity)
        {
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();
        }

        IEnumerable<T> IRepository<T>.GetAll(Expression<Func<T, bool>> filter)
        {
                return filter == null
                    ? _context.Set<T>().ToList()
                    : _context.Set<T>().Where(filter).ToList();
        }
    }
}