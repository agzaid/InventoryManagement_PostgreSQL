using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Application.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public PermissionService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<RolePermissionDto> GetRolePermissionsAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                throw new NotFoundException($"Role with ID {roleId} not found");

            var rolePermissions = await _unitOfWork.RolePermissionRepository.GetAllAsyncExpression(
                rp => rp.RoleId == roleId,
                orderBy: rp => rp.Id
            );

            var allPermissions = GetAllPermissionsFromConstants();
            var assignedPermissionCodes = rolePermissions.Select(rp => rp.PermissionCode).ToHashSet();

            var permissionDtos = allPermissions.Select(p => new PermissionDto
            {
                Code = p.Code,
                Name = p.Name,
                Module = p.Module,
                DisplayName = p.DisplayName,
                IsChecked = assignedPermissionCodes.Contains(p.Code)
            }).ToList();

            return new RolePermissionDto
            {
                RoleId = role.Id,
                RoleName = role.Name ?? string.Empty,
                RoleDescription = role.Description ?? string.Empty,
                Permissions = permissionDtos
            };
        }

        public async Task<List<PermissionDto>> GetAllPermissionsAsync()
        {
            return GetAllPermissionsFromConstants().Select(p => new PermissionDto
            {
                Code = p.Code,
                Name = p.Name,
                Module = p.Module,
                DisplayName = p.DisplayName,
                IsChecked = false
            }).ToList();
        }

        public async Task<bool> UpdateRolePermissionsAsync(UpdateRolePermissionsRequest request)
        {
            try
            {
                // Remove existing permissions for the role
                var existingPermissions = await _unitOfWork.RolePermissionRepository.GetAllAsyncExpression(
                    rp => rp.RoleId == request.RoleId,
                    orderBy: rp => rp.Id
                );

                foreach (var permission in existingPermissions)
                {
                    await _unitOfWork.RolePermissionRepository.DeleteAsync(permission);
                }

                // Add new permissions
                var allPermissions = GetAllPermissionsFromConstants();
                var newPermissions = request.PermissionCodes.Select(code =>
                {
                    var permissionInfo = allPermissions.First(p => p.Code == code);
                    return new RolePermission
                    {
                        RoleId = request.RoleId,
                        PermissionCode = code,
                        PermissionName = permissionInfo.Name,
                        Module = permissionInfo.Module,
                        IsAllowed = true,
                        CreatedAt = DateTime.UtcNow
                    };
                }).ToList();

                foreach (var permission in newPermissions)
                {
                    await _unitOfWork.RolePermissionRepository.AddAsync(permission);
                }

                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> HasPermissionAsync(string userId, string permissionCode)
        {
            // Check if user is Admin (automatic access to everything)
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
                return true;

            // Check role-based permissions
            return await HasRolePermissionAsync(userId, permissionCode);
        }

        public async Task<bool> HasRolePermissionAsync(string userId, string permissionCode)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Any())
                return false;

            // Check if any of the user's roles have the required permission
            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var rolePermissions = await _unitOfWork.RolePermissionRepository.GetAllAsyncExpression(
                        rp => rp.RoleId == role.Id && rp.PermissionCode == permissionCode && rp.IsAllowed,
                        orderBy: rp => rp.Id
                    );

                    if (rolePermissions.Any())
                        return true;
                }
            }

            return false;
        }

        public async Task<List<string>> GetUserPermissionsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new List<string>();

            var userRoles = await _userManager.GetRolesAsync(user);
            var permissions = new HashSet<string>();

            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var rolePermissions = await _unitOfWork.RolePermissionRepository.GetAllAsyncExpression(
                        rp => rp.RoleId == role.Id && rp.IsAllowed,
                        orderBy: rp => rp.Id
                    );

                    foreach (var permission in rolePermissions)
                    {
                        permissions.Add(permission.PermissionCode);
                    }
                }
            }

            return permissions.ToList();
        }

        private List<(string Code, string Name, string Module, string DisplayName)> GetAllPermissionsFromConstants()
        {
            return new List<(string, string, string, string)>
            {
                // Daily Movement Module
                (SystemPermissions.PROG01, "تسجيل حركة الوارد", "Daily Movement", "تسجيل حركة الوارد"),
                (SystemPermissions.PROG02, "تسجيل حركة المنصرف", "Daily Movement", "تسجيل حركة المنصرف"),
                (SystemPermissions.PROG03, "تسجيل حركة التحويل", "Daily Movement", "تسجيل حركة التحويل"),
                (SystemPermissions.PROG11, "تسجيل المرتجعات", "Daily Movement", "تسجيل المرتجعات"),
                (SystemPermissions.PROG12, "استعلام وبحث البيانات", "Daily Movement", "استعلام وبحث البيانات"),

                // Inventory Module
                (SystemPermissions.PROG13, "تسجيل ارصده الفتح", "Inventory", "تسجيل ارصده الفتح"),
                (SystemPermissions.PROG14, "الأرصدة الحالية", "Inventory", "الأرصدة الحالية"),
                (SystemPermissions.PROG21, "كارت الصنف", "Inventory", "كارت الصنف"),
                (SystemPermissions.PROG22, "جرد المخزن", "Inventory", "جرد المخزن"),
                (SystemPermissions.PROG23, "تقارير الارصده والاستهلاك", "Inventory", "تقارير الارصده والاستهلاك"),
                (SystemPermissions.PROG24, "جرد كل المخازن", "Inventory", "جرد كل المخازن"),

                // System Codes Module
                (SystemPermissions.PROG25, "أكواد الأصناف", "System Codes", "أكواد الأصناف"),
                (SystemPermissions.PROG29, "أكواد الموردين", "System Codes", "أكواد الموردين"),
                (SystemPermissions.PROG31, "ادارات البورصه", "System Codes", "ادارات البورصه"),
                (SystemPermissions.PROG32, "العاملين بالبورصه", "System Codes", "العاملين بالبورصه"),

                // Admin Module
                (SystemPermissions.PROG33, "صلاحيات المستخدمين", "Admin", "صلاحيات المستخدمين"),
                (SystemPermissions.PROG34, "تهيئه اعدادات النظام", "Admin", "تهيئه اعدادات النظام"),
                (SystemPermissions.PROG35, "ترحيل الحركه اليوميه", "Admin", "ترحيل الحركه اليوميه")
            };
        }
    }
}
