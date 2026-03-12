using Application.Interfaces.Models;

namespace Application.Interfaces.Contracts.Service
{
    public interface IPermissionService
    {
        Task<RolePermissionDto> GetRolePermissionsAsync(string roleId);
        Task<List<PermissionDto>> GetAllPermissionsAsync();
        Task<bool> UpdateRolePermissionsAsync(UpdateRolePermissionsRequest request);
        Task<bool> HasPermissionAsync(string userId, string permissionCode);
        Task<bool> HasRolePermissionAsync(string userId, string permissionCode);
        Task<List<string>> GetUserPermissionsAsync(string userId);
    }
}
