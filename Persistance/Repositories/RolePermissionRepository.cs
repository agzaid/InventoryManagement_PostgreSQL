using Application.Interfaces.Contracts.Persistance;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Repositories
{
    public class RolePermissionRepository : GenericRepository<RolePermission>, IRolePermissionRepository
    {
        private readonly InventoryManagementDbContext _context;

        public RolePermissionRepository(InventoryManagementDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolePermission>> GetByRoleIdAsync(string roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RolePermission>> GetByPermissionCodeAsync(string permissionCode)
        {
            return await _context.RolePermissions
                .Where(rp => rp.PermissionCode == permissionCode)
                .ToListAsync();
        }

        public async Task<bool> HasPermissionAsync(string roleId, string permissionCode)
        {
            return await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && 
                               rp.PermissionCode == permissionCode && 
                               rp.IsAllowed);
        }
    }
}
