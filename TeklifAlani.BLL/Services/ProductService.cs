using System.Collections.Generic;
using System.Linq.Expressions;
using TeklifAlani.BLL.Abstract;
using TeklifAlani.Core.Interfaces;
using TeklifAlani.Core.Models;
using TeklifAlani.DAL.Abstract;

namespace TeklifAlani.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductDal _productDal;

        public ProductService(IProductDal productDal)
        {
            _productDal = productDal;
        }

        public void Create(Product entity)
        {
            _productDal.Create(entity);
        }

        public void Delete(Product entity)
        {
            _productDal.Delete(entity);
        }

        public List<Product> GetAll(Expression<Func<Product, bool>> filter = null)
        {
            return _productDal.GetAll(filter);
        }

        public Product GetOne(Expression<Func<Product, bool>> filter = null)
        {
            return _productDal.GetOne(filter);
        }

        public Product GetById(int id)
        {
            return _productDal.GetById(id);
        }

        public void Update(Product entity)
        {
            _productDal.Update(entity);
        }

        public Task<List<Product>> SearchProducts(string query)
        {
            return _productDal.SearchProducts(query);
        }
    }
}
