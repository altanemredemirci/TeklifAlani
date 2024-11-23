using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifAlani.Core.Models;
using TeklifAlani.DAL.Abstract;
using TeklifAlani.DAL.Context;
using TeklifAlani.WEBUI.Context;

namespace TeklifAlani.DAL.Concrete
{
    public class EfCoreCityDal : EfCoreGenericRepositoryDal<City, ApplicationIdentityDbContext>, ICityDal
    {
        private readonly ApplicationIdentityDbContext _context;

        public EfCoreCityDal(ApplicationIdentityDbContext context) : base(context)
        {
            _context = context;
        }

        public List<District> GetDistrictsByCityId(int id)
        {
            return _context.Cities.Include(i => i.Districts).Where(i => i.Id == id).FirstOrDefault().Districts;
        }
    }
}
