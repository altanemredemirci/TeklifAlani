using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifAlani.Core.Models;

namespace TeklifAlani.BLL.Abstract
{
    public interface IProductService:IRepositoryService<Product>
    {
        Task<List<Product>> SearchProducts(string query);
    }
}
