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
    public class CityService : ICityService
    {
        private ICityDal _cityDal;

        public CityService(ICityDal cityDal)
        {
            _cityDal = cityDal;
        }

        public void Create(City entity)
        {
            _cityDal.Create(entity);
        }

        public void Delete(City entity)
        {
            _cityDal.Delete(entity);
        }

        public List<City> GetAll(Expression<Func<City, bool>> filter = null)
        {
            return _cityDal.GetAll(filter);
        }

        public City GetById(int id)
        {
            return _cityDal.GetById(id);
        }

        public List<District> GetDistrictsByCityId(int id)
        {
            return _cityDal.GetDistrictsByCityId(id);
        }

        public City GetOne(Expression<Func<City, bool>> filter = null)
        {
            return _cityDal.GetOne(filter);
        }

        public void Update(City entity)
        {
            _cityDal.Update(entity);
        }
    }
}
