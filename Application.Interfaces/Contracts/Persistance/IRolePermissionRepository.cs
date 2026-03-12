using Application.Interfaces.Models;
using Domain.Entities;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface IRolePermissionRepository : IGenericRepository<RolePermission>
    {
        Task<IEnumerable<RolePermission>> GetByRoleIdAsync(string roleId);
        Task<IEnumerable<RolePermission>> GetByPermissionCodeAsync(string permissionCode);
        Task<bool> HasPermissionAsync(string roleId, string permissionCode);
    }
}
