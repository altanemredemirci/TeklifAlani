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
    public class EfCoreBrandDal : EfCoreGenericRepositoryDal<Brand, ApplicationIdentityDbContext>, IBrandDal
    {
        private readonly ApplicationIdentityDbContext _context;

        public EfCoreBrandDal(ApplicationIdentityDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
