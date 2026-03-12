using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Contracts.Persistance;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly InventoryManagementDbContext _dbContext;

        public UserRoleRepository(InventoryManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<UserRole>> GetRolesByUserIdAsync(string userId)
        {
            return await _dbContext.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .ToListAsync();
        }

        public async Task<IList<UserRole>> GetUsersByRoleIdAsync(string roleId)
        {
            return await _dbContext.UserRoles
                // Removed User navigation - using ASP.NET Identity
                // .Include(ur => ur.User)
                .Where(ur => ur.RoleId == roleId)
                .ToListAsync();
        }

        public async Task AddUserRoleAsync(UserRole userRole)
        {
            await _dbContext.UserRoles.AddAsync(userRole);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveUserRoleAsync(string userId, string roleId)
        {
            var userRole = await _dbContext.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            
            if (userRole != null)
            {
                _dbContext.UserRoles.Remove(userRole);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> IsUserInRoleAsync(string userId, string roleId)
        {
            return await _dbContext.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        }
    }
}
