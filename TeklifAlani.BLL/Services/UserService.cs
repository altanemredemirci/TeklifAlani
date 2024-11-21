using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifAlani.Core.Identity;
using TeklifAlani.DAL.Context;

namespace TeklifAlani.BLL.Services
{
    public class UserService
    {
        private readonly ApplicationIdentityDbContext _context;

        public UserService(ApplicationIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }
    }
}
