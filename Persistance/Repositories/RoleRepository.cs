using Application.Interfaces.Contracts.Persistance;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Repositories
{
    public class RoleRepository : GenericRepository<ApplicationRole>, IRoleRepository
    {
        private readonly InventoryManagementDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleRepository(InventoryManagementDbContext context, RoleManager<ApplicationRole> roleManager) : base(context)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<ApplicationRole?> GetByNameAsync(string name)
        {
            return await _roleManager.FindByNameAsync(name);
        }

        public async Task<IEnumerable<ApplicationRole>> GetByUserIdAsync(string userId)
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            return await _context.Roles
                .Where(r => userRoles.Contains(r.Id))
                .ToListAsync();
        }
    }
}
