using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeklifAlani.Core.Identity;
using TeklifAlani.Core.Models;

namespace TeklifAlani.DAL.Context
{
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
        : base(options)
        {
        }

       
    }
}
