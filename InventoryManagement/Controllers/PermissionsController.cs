using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Application.Interfaces.Models;
using Application.Interfaces.Contracts.Service;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PermissionsController : BaseController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRoleManagementService _roleManagementService;
        private readonly ILogger<PermissionsController> _logger;

        public PermissionsController(
            RoleManager<ApplicationRole> roleManager,
            IRoleManagementService roleManagementService,
            ILogger<PermissionsController> logger)
        {
            _roleManager = roleManager;
            _roleManagementService = roleManagementService;
            _logger = logger;
        }

        // GET: Permissions
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var model = new AllPermissionsViewModel
            {
                Roles = new List<RolePermissionSummaryViewModel>()
            };

            foreach (var role in roles)
            {
                var permissionCount = await _roleManagementService.GetRolePermissionCountAsync(role.Id);

                model.Roles.Add(new RolePermissionSummaryViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name ?? string.Empty,
                    Level = role.Level,
                    PermissionCount = permissionCount
                });
            }

            // Get all available permissions from service
            var allPermissions = await _roleManagementService.GetAllDistinctPermissionsAsync();

            model.PermissionGroups = allPermissions
                .GroupBy(p => p.Module)
                .Select(g => new PermissionGroupViewModel
                {
                    ModuleName = g.Key,
                    Permissions = g.Select(p => new PermissionItemViewModel
                    {
                        Code = p.PermissionCode,
                        Name = p.PermissionName,
                        Module = p.Module,
                        IsAllowed = false
                    }).ToList()
                }).ToList();

            return View(model);
        }

        // POST: Permissions/SeedPermissions
        // Note: Permissions are now seeded automatically on application startup
        // This endpoint is kept for manual re-seeding if needed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SeedPermissions()
        {
            TempData["Info"] = "الصلاحيات يتم تحميلها تلقائياً عند بدء التطبيق";
            return RedirectToActionWithCulture(nameof(Index));
        }

        // GET: Permissions/ManageRole/5
        public async Task<IActionResult> ManageRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var existingPermissions = await _roleManagementService.GetRolePermissionsAsync(id);

            // Get all available permissions from service
            var allPermissions = await _roleManagementService.GetAllDistinctPermissionsAsync();

            var permissionGroups = new List<PermissionGroupViewModel>();

            foreach (var group in allPermissions.GroupBy(p => p.Module))
            {
                var groupPermissions = new List<PermissionItemViewModel>();
                
                foreach (var permission in group)
                {
                    var existing = existingPermissions.FirstOrDefault(ep => ep.PermissionCode == permission.PermissionCode);
                    groupPermissions.Add(new PermissionItemViewModel
                    {
                        Id = existing?.Id,
                        Code = permission.PermissionCode,
                        Name = permission.PermissionName,
                        Module = permission.Module,
                        IsAllowed = existing?.IsAllowed ?? false
                    });
                }

                permissionGroups.Add(new PermissionGroupViewModel
                {
                    ModuleName = group.Key,
                    Permissions = groupPermissions
                });
            }

            var model = new ManageRolePermissionsViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name ?? "",
                PermissionGroups = permissionGroups
            };

            return View(model);
        }

        // POST: Permissions/ManageRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRole(string roleId, List<string> permissionCodes)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }

            await _roleManagementService.UpdateRolePermissionsAsync(roleId, permissionCodes ?? new List<string>());

            TempData["Success"] = "تم تحديث صلاحيات الدور بنجاح";
            return RedirectToActionWithCulture(nameof(Index));
        }

        // Temporary action to seed permissions for existing roles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SeedRolePermissions()
        {
            try
            {
                var adminRole = await _roleManager.FindByNameAsync("Admin");
                if (adminRole != null)
                {
                    // Check if permissions already exist
                    var existingCount = await _roleManagementService.GetRolePermissionCountAsync(adminRole.Id);

                    if (existingCount == 0)
                    {
                        await _roleManagementService.SeedAllPermissionsToAdminAsync(adminRole.Id);
                        var allPermissionsCount = (await _roleManagementService.GetSystemPermissionsAsync()).Count;
                        TempData["Success"] = $"تم إضافة {allPermissionsCount} صلاحية لدور Admin بنجاح";
                    }
                    else
                    {
                        TempData["Info"] = "الصلاحيات موجودة بالفعل لدور Admin";
                    }
                }

                return RedirectToActionWithCulture(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding role permissions");
                TempData["Error"] = "حدث خطأ أثناء إضافة الصلاحيات";
                return RedirectToActionWithCulture(nameof(Index));
            }
        }
    }
}
