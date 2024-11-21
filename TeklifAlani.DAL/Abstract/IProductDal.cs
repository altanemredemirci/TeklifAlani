using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifAlani.Core.Interfaces;
using TeklifAlani.Core.Models;

namespace TeklifAlani.DAL.Abstract
{
    public interface IProductDal:IRepository<Product>
    {
        Task<List<Product>> SearchProducts(string query);
        
    }
}
