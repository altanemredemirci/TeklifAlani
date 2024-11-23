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
    public class EfCoreProductDal : EfCoreGenericRepositoryDal<Product, ApplicationIdentityDbContext>, IProductDal
    {
        private readonly ApplicationIdentityDbContext _context;

        public EfCoreProductDal(ApplicationIdentityDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Product>> SearchProducts(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Product>();

                query = $"%{query}%"; 

                var results = await _context.Products
                    .Include(p => p.Brand) 
                    .Where(p =>
                        EF.Functions.Like(p.Brand.Name, query) || 
                        EF.Functions.Like(p.Description, query) || 
                        EF.Functions.Like(p.ProductCode, query)) 
                    .ToListAsync();

                return results;
            
           
        }
    }
}
