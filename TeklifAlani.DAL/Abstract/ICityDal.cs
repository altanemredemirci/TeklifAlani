using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifAlani.Core.Interfaces;
using TeklifAlani.Core.Models;

namespace TeklifAlani.DAL.Abstract
{
    public interface ICityDal:IRepository<City>
    {
        public List<District> GetDistrictsByCityId(int id);
    }
}
