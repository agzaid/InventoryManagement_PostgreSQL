using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Contracts.Service;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Service
{
    public class RoleManagementService : IRoleManagementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<RolePermission>> GetSystemPermissionsAsync()
        {
            var result = await _unitOfWork.RolePermissionRepository.GetAllAsyncExpression(
                rp => rp.RoleId == null,
                orderBy: rp => rp.Module
            );
            return result.ToList();
        }

        public async Task<List<RolePermission>> GetRolePermissionsAsync(string roleId)
        {
            var result = await _unitOfWork.RolePermissionRepository.GetAllAsyncExpression(
                rp => rp.RoleId == roleId && rp.IsAllowed,
                orderBy: rp => rp.Module
            );
            return result.ToList();
        }

        public async Task<List<RolePermission>> GetPermissionsGroupedByModuleAsync()
        {
            var result = await _unitOfWork.RolePermissionRepository.GetAllAsyncExpression(
                rp => rp.RoleId == null,
                orderBy: rp => rp.Module
            );
            return result.ToList();
        }

        public async Task<List<RolePermission>> GetAllDistinctPermissionsAsync()
        {
            var allPermissions = await _unitOfWork.RolePermissionRepository.GetAllAsyncExpression(
                rp => true,
                orderBy: rp => rp.Module
            );
            
            // Get distinct permissions by PermissionCode
            return allPermissions
                .GroupBy(rp => rp.PermissionCode)
                .Select(g => g.First())
                .OrderBy(rp => rp.Module)
                .ThenBy(rp => rp.PermissionCode)
                .ToList();
        }

        public async Task UpdateRolePermissionsAsync(string roleId, List<string> permissionCodes)
        {
            // Remove existing permissions for the role
            var existingPermissions = await _unitOfWork.RolePermissionRepository.GetAllAsyncExpression(
                rp => rp.RoleId == roleId,
                orderBy: rp => rp.Id,
                tracked: true
            );

            foreach (var permission in existingPermissions)
            {
                await _unitOfWork.RolePermissionRepository.DeleteAsync(permission);
            }

            // Add new permissions if any selected
            if (permissionCodes != null && permissionCodes.Any())
            {
                // Get system-defined permission templates
                var systemPermissions = await _unitOfWork.RolePermissionRepository.GetAllAsyncExpression(
                    rp => rp.RoleId == null,
                    orderBy: rp => rp.Id
                );

                foreach (var permCode in permissionCodes)
                {
                    var template = systemPermissions.FirstOrDefault(p => p.PermissionCode == permCode);
                    if (template != null)
                    {
                        var rolePermission = new RolePermission
                        {
                            RoleId = roleId,
                            PermissionCode = template.PermissionCode,
                            PermissionName = template.PermissionName,
                            Module = template.Module,
                            IsAllowed = true,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.RolePermissionRepository.AddAsync(rolePermission);
                    }
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task<int> GetRolePermissionCountAsync(string roleId)
        {
            var permissions = await _unitOfWork.RolePermissionRepository.GetAllAsyncExpression(
                rp => rp.RoleId == roleId && rp.IsAllowed,
                orderBy: rp => rp.Id
            );
            return permissions.Count;
        }

        public async Task DeleteRolePermissionsAsync(string roleId)
        {
            var rolePermissions = await _unitOfWork.RolePermissionRepository.GetAllAsyncExpression(
                rp => rp.RoleId == roleId,
                orderBy: rp => rp.Id,
                tracked: true
            );

            foreach (var permission in rolePermissions)
            {
                await _unitOfWork.RolePermissionRepository.DeleteAsync(permission);
            }

            await _unitOfWork.CompleteAsync();
        }

        public Dictionary<string, string> GetModuleColors()
        {
            return new Dictionary<string, string>
            {
                { "الحركة اليومية", "yellow" },
                { "إدارة المخزون", "blue" },
                { "أكواد النظام", "purple" },
                { "إدارة النظام", "red" }
            };
        }

        public async Task SeedAllPermissionsToAdminAsync(string adminRoleId)
        {
            // Get all system-defined permission templates
            var allPermissions = await _unitOfWork.RolePermissionRepository.GetAllAsyncExpression(
                rp => rp.RoleId == null,
                orderBy: rp => rp.Id
            );

            // Create role permissions for Admin
            var rolePermissions = allPermissions.Select(p => new RolePermission
            {
                RoleId = adminRoleId,
                PermissionCode = p.PermissionCode,
                PermissionName = p.PermissionName,
                Module = p.Module,
                IsAllowed = true,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            foreach (var permission in rolePermissions)
            {
                await _unitOfWork.RolePermissionRepository.AddAsync(permission);
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
