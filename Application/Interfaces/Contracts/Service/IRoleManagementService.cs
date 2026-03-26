using Domain.Entities;

namespace Application.Interfaces.Contracts.Service
{
    public interface IRoleManagementService
    {
        // Permission operations
        Task<List<RolePermission>> GetSystemPermissionsAsync();
        Task<List<RolePermission>> GetRolePermissionsAsync(string roleId);
        Task<List<RolePermission>> GetPermissionsGroupedByModuleAsync();
        Task<List<RolePermission>> GetAllDistinctPermissionsAsync();
        Task UpdateRolePermissionsAsync(string roleId, List<string> permissionCodes);
        Task DeleteRolePermissionsAsync(string roleId);
        Dictionary<string, string> GetModuleColors();
        
        // Role operations
        Task<int> GetRolePermissionCountAsync(string roleId);
        Task SeedAllPermissionsToAdminAsync(string adminRoleId);
    }
}
