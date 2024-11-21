using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifAlani.Core.Models;
using TeklifAlani.DAL.Abstract;
using TeklifAlani.DAL.Context;

namespace TeklifAlani.DAL.Concrete
{
    public class EfCoreProductDal : EfCoreGenericRepository<Product, ApplicationIdentityDbContext>,IProductDal
    {
      
    }
}
