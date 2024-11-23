using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TeklifAlani.BLL.Abstract;
using TeklifAlani.Core.Models;
using TeklifAlani.DAL.Abstract;

namespace TeklifAlani.BLL.Services
{
    public class BrandService : IBrandService
    {
        private IBrandDal _brandDal;

        public BrandService(IBrandDal brandDal)
        {
            _brandDal = brandDal;
        }
        public void Create(Brand entity)
        {
            _brandDal.Create(entity);
        }

        public void Delete(Brand entity)
        {
            _brandDal.Delete(entity);
        }

        public List<Brand> GetAll(Expression<Func<Brand, bool>> filter = null)
        {
            return _brandDal.GetAll(filter);
        }

        public Brand GetById(int id)
        {
            return _brandDal.GetById(id);
        }

        public Brand GetOne(Expression<Func<Brand, bool>> filter = null)
        {
            return _brandDal.GetOne(filter);
        }

        public void Update(Brand entity)
        {
            _brandDal.Update(entity);
        }
    }
}
