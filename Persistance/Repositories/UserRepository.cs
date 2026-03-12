using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Contracts.Persistance;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Persistance.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly InventoryManagementDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(InventoryManagementDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        // Note: Generic repository methods removed since ApplicationUser is managed by ASP.NET Identity
        // Use UserManager for CRUD operations on ApplicationUser
    }
}
