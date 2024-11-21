using System.Collections.Generic;
using System.Linq.Expressions;
using TeklifAlani.BLL.Abstract;
using TeklifAlani.Core.Interfaces;
using TeklifAlani.Core.Models;

namespace TeklifAlani.BLL.Services
{
    public class ProductService : IProductService
    {
        public void Create(Product entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Product entity)
        {
            throw new NotImplementedException();
        }

        public List<Product> GetAll(Expression<Func<Product, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public Product GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Product GetOne(Expression<Func<Product, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public void Update(Product entity)
        {
            throw new NotImplementedException();
        }
    }
}
