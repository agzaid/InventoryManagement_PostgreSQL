using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Application.Interfaces.Models;
using Persistance;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PermissionsController : BaseController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly InventoryManagementDbContext _context;
        private readonly ILogger<PermissionsController> _logger;

        public PermissionsController(
            RoleManager<ApplicationRole> roleManager,
            InventoryManagementDbContext context,
            ILogger<PermissionsController> logger)
        {
            _roleManager = roleManager;
            _context = context;
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
                var permissionCount = await _context.RolePermissions
                    .Where(rp => rp.RoleId == role.Id && rp.IsAllowed)
                    .CountAsync();

                model.Roles.Add(new RolePermissionSummaryViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name ?? string.Empty,
                    Level = role.Level,
                    PermissionCount = permissionCount
                });
            }

            var allPermissions = GetAllPermissions();
            model.PermissionGroups = allPermissions
                .GroupBy(p => p.Module)
                .Select(g => new PermissionGroupViewModel
                {
                    ModuleName = g.Key,
                    Permissions = g.Select(p => new PermissionItemViewModel
                    {
                        Code = p.Code,
                        Name = p.Name,
                        Module = p.Module,
                        Controller = p.Controller,
                        Action = p.Action,
                        Description = p.Description,
                        IsAllowed = false
                    }).ToList()
                }).ToList();

            return View(model);
        }

        // GET: Permissions/ManagePermissionActions
        public async Task<IActionResult> ManagePermissionActions()
        {
            var permissionActions = await _context.PermissionActions
                .OrderBy(pa => pa.Module)
                .ThenBy(pa => pa.PermissionName)
                .ToListAsync();

            var model = new PermissionActionManagementDto
            {
                PermissionActions = permissionActions.Select(pa => new PermissionActionDto
                {
                    Id = pa.Id,
                    PermissionCode = pa.PermissionCode,
                    PermissionName = pa.PermissionName,
                    Module = pa.Module,
                    Controller = pa.Controller,
                    Action = pa.Action,
                    FullUrl = pa.FullUrl,
                    HttpMethod = pa.HttpMethod,
                    Description = pa.Description,
                    IsActive = pa.IsActive
                }).ToList(),
                AvailablePermissions = GetAvailablePermissionsGrouped(),
                AvailableControllers = GetAvailableControllers()
            };

            return View(model);
        }

        // POST: Permissions/CreatePermissionAction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePermissionAction(PermissionActionDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "البيانات المدخلة غير صحيحة";
                return RedirectToActionWithCulture(nameof(ManagePermissionActions));
            }

            var exists = await _context.PermissionActions
                .AnyAsync(pa => pa.PermissionCode == model.PermissionCode && 
                               pa.Controller == model.Controller && 
                               pa.Action == model.Action);

            if (exists)
            {
                TempData["Error"] = "هذا الربط موجود بالفعل";
                return RedirectToActionWithCulture(nameof(ManagePermissionActions));
            }

            var permissionAction = new PermissionAction
            {
                PermissionCode = model.PermissionCode,
                PermissionName = model.PermissionName,
                Module = model.Module,
                Controller = model.Controller,
                Action = model.Action,
                FullUrl = $"/{model.Controller}/{model.Action}",
                HttpMethod = model.HttpMethod ?? "GET",
                Description = model.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.PermissionActions.Add(permissionAction);
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم إضافة الربط بنجاح";
            return RedirectToActionWithCulture(nameof(ManagePermissionActions));
        }

        // POST: Permissions/DeletePermissionAction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePermissionAction(int id)
        {
            var permissionAction = await _context.PermissionActions.FindAsync(id);
            if (permissionAction == null)
            {
                TempData["Error"] = "الربط غير موجود";
                return RedirectToActionWithCulture(nameof(ManagePermissionActions));
            }

            _context.PermissionActions.Remove(permissionAction);
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم حذف الربط بنجاح";
            return RedirectToActionWithCulture(nameof(ManagePermissionActions));
        }

        // POST: Permissions/TogglePermissionAction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePermissionAction(int id)
        {
            var permissionAction = await _context.PermissionActions.FindAsync(id);
            if (permissionAction == null)
            {
                TempData["Error"] = "الربط غير موجود";
                return RedirectToActionWithCulture(nameof(ManagePermissionActions));
            }

            permissionAction.IsActive = !permissionAction.IsActive;
            permissionAction.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var status = permissionAction.IsActive ? "تفعيل" : "تعطيل";
            TempData["Success"] = $"تم {status} الربط بنجاح";
            return RedirectToActionWithCulture(nameof(ManagePermissionActions));
        }

        // GET: Permissions/ManageRole/5
        public async Task<IActionResult> ManageRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var existingPermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == id)
                .ToListAsync();

            var allPermissions = GetAllPermissions();
            var permissionGroups = new List<PermissionGroupViewModel>();

            foreach (var group in allPermissions.GroupBy(p => p.Module))
            {
                var groupPermissions = new List<PermissionItemViewModel>();
                
                foreach (var permission in group)
                {
                    var existing = existingPermissions.FirstOrDefault(ep => ep.PermissionCode == permission.Code);
                    groupPermissions.Add(new PermissionItemViewModel
                    {
                        Id = existing?.Id,
                        Code = permission.Code,
                        Name = permission.Name,
                        Module = permission.Module,
                        Controller = permission.Controller,
                        Action = permission.Action,
                        Description = permission.Description,
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

            var existingPermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(existingPermissions);

            if (permissionCodes != null && permissionCodes.Any())
            {
                var allPermissions = GetAllPermissions();
                
                foreach (var code in permissionCodes)
                {
                    var permissionInfo = allPermissions.FirstOrDefault(p => p.Code == code);
                    if (permissionInfo != null)
                    {
                        _context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = roleId,
                            PermissionCode = code,
                            PermissionName = permissionInfo.Name,
                            Module = permissionInfo.Module,
                            IsAllowed = true,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "تم تحديث صلاحيات الدور بنجاح";
            return RedirectToActionWithCulture(nameof(Index));
        }

        private List<PermissionViewModel> GetAllPermissions()
        {
            return new List<PermissionViewModel>
            {
                // Daily Movement Module
                new PermissionViewModel { Code = "PROG01", Name = "تسجيل حركة الوارد", Module = "الحركة اليومية", Controller = "InventoryMovement", Action = "Incoming", Description = "إضافة وتسجيل حركات الوارد للمخزن" },
                new PermissionViewModel { Code = "PROG02", Name = "تسجيل حركة المنصرف", Module = "الحركة اليومية", Controller = "InventoryMovement", Action = "Outgoing", Description = "إضافة وتسجيل حركات المنصرف من المخزن" },
                new PermissionViewModel { Code = "PROG03", Name = "تسجيل حركة التحويل", Module = "الحركة اليومية", Controller = "InventoryMovement", Action = "Transfer", Description = "تحويل الأصناف بين المخازن" },
                new PermissionViewModel { Code = "PROG11", Name = "تسجيل المرتجعات", Module = "الحركة اليومية", Controller = "InventoryMovement", Action = "Returns", Description = "تسجيل المرتجعات والإرجاعات" },
                new PermissionViewModel { Code = "PROG12", Name = "استعلام وبحث البيانات", Module = "الحركة اليومية", Controller = "InventoryMovement", Action = "SearchAndInquiry", Description = "البحث والاستعلام عن الحركات" },

                // Inventory Module
                new PermissionViewModel { Code = "PROG13", Name = "تسجيل أرصدة الفتح", Module = "إدارة المخزون", Controller = "Inventory", Action = "OpeningBalance", Description = "تسجيل الأرصدة الافتتاحية" },
                new PermissionViewModel { Code = "PROG14", Name = "الأرصدة الحالية", Module = "إدارة المخزون", Controller = "Inventory", Action = "CurrentStock", Description = "عرض الأرصدة الحالية للأصناف" },
                new PermissionViewModel { Code = "PROG21", Name = "كارت الصنف", Module = "إدارة المخزون", Controller = "Inventory", Action = "ItemCard", Description = "عرض كارت الصنف وحركاته" },
                new PermissionViewModel { Code = "PROG22", Name = "جرد المخزن", Module = "إدارة المخزون", Controller = "Inventory", Action = "StockTaking", Description = "إجراء جرد المخزن" },
                new PermissionViewModel { Code = "PROG23", Name = "تقارير الأرصدة والاستهلاك", Module = "إدارة المخزون", Controller = "Reports", Action = "StockReports", Description = "تقارير الأرصدة والاستهلاك" },
                new PermissionViewModel { Code = "PROG24", Name = "جرد كل المخازن", Module = "إدارة المخزون", Controller = "Inventory", Action = "AllStoresTaking", Description = "جرد شامل لجميع المخازن" },

                // System Codes Module
                new PermissionViewModel { Code = "PROG25", Name = "أكواد الأصناف", Module = "أكواد النظام", Controller = "Items", Action = "Index", Description = "إدارة أكواد الأصناف" },
                new PermissionViewModel { Code = "PROG29", Name = "أكواد الموردين", Module = "أكواد النظام", Controller = "Suppliers", Action = "Index", Description = "إدارة أكواد الموردين" },
                new PermissionViewModel { Code = "PROG31", Name = "إدارات البورصة", Module = "أكواد النظام", Controller = "Departments", Action = "Index", Description = "إدارة الإدارات والأقسام" },
                new PermissionViewModel { Code = "PROG32", Name = "العاملين بالبورصة", Module = "أكواد النظام", Controller = "Employees", Action = "Index", Description = "إدارة بيانات العاملين" },

                // Admin Module
                new PermissionViewModel { Code = "PROG33", Name = "صلاحيات المستخدمين", Module = "إدارة النظام", Controller = "UserManagement", Action = "Index", Description = "إدارة المستخدمين والصلاحيات" },
                new PermissionViewModel { Code = "PROG34", Name = "تهيئة إعدادات النظام", Module = "إدارة النظام", Controller = "Settings", Action = "Index", Description = "إعدادات النظام العامة" },
                new PermissionViewModel { Code = "PROG35", Name = "ترحيل الحركة اليومية", Module = "إدارة النظام", Controller = "System", Action = "PostDailyMovement", Description = "ترحيل الحركات اليومية" },
            };
        }

        private List<PermissionGroupViewModel> GetAllPermissionGroups()
        {
            var allPermissions = GetAllPermissions();
            var groups = new List<PermissionGroupViewModel>();

            foreach (var group in allPermissions.GroupBy(p => p.Module))
            {
                groups.Add(new PermissionGroupViewModel
                {
                    ModuleName = group.Key,
                    Permissions = group.Select(p => new PermissionItemViewModel
                    {
                        Code = p.Code,
                        Name = p.Name,
                        Module = p.Module,
                        Controller = p.Controller,
                        Action = p.Action,
                        Description = p.Description,
                        IsAllowed = false
                    }).ToList()
                });
            }

            return groups;
        }

        private List<PermissionModuleDto> GetAvailablePermissionsGrouped()
        {
            var allPermissions = GetAllPermissions();
            var modules = new List<PermissionModuleDto>();

            var moduleGroups = allPermissions.GroupBy(p => p.Module);
            foreach (var group in moduleGroups)
            {
                modules.Add(new PermissionModuleDto
                {
                    Module = group.Key,
                    ModuleNameArabic = group.Key,
                    Permissions = group.Select(p => new PermissionItemDto
                    {
                        Code = p.Code,
                        Name = p.Name,
                        IsAssigned = false
                    }).ToList()
                });
            }

            return modules;
        }

        private List<ControllerActionDto> GetAvailableControllers()
        {
            return new List<ControllerActionDto>
            {
                new ControllerActionDto { Controller = "InventoryMovement", Action = "Incoming", FullPath = "/InventoryMovement/Incoming", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "InventoryMovement", Action = "Outgoing", FullPath = "/InventoryMovement/Outgoing", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "InventoryMovement", Action = "Transfer", FullPath = "/InventoryMovement/Transfer", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "InventoryMovement", Action = "Returns", FullPath = "/InventoryMovement/Returns", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "InventoryMovement", Action = "SearchAndInquiry", FullPath = "/InventoryMovement/SearchAndInquiry", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "Inventory", Action = "OpeningBalance", FullPath = "/Inventory/OpeningBalance", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "Inventory", Action = "CurrentStock", FullPath = "/Inventory/CurrentStock", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "Inventory", Action = "ItemCard", FullPath = "/Inventory/ItemCard", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "Inventory", Action = "StockTaking", FullPath = "/Inventory/StockTaking", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "Reports", Action = "StockReports", FullPath = "/Reports/StockReports", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "Inventory", Action = "AllStoresTaking", FullPath = "/Inventory/AllStoresTaking", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "Items", Action = "Index", FullPath = "/Items/Index", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "Suppliers", Action = "Index", FullPath = "/Suppliers/Index", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "Departments", Action = "Index", FullPath = "/Departments/Index", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "Employees", Action = "Index", FullPath = "/Employees/Index", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "UserManagement", Action = "Index", FullPath = "/UserManagement/Index", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "Settings", Action = "Index", FullPath = "/Settings/Index", HttpMethod = "GET" },
                new ControllerActionDto { Controller = "System", Action = "PostDailyMovement", FullPath = "/System/PostDailyMovement", HttpMethod = "GET" }
            };
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
                    var existingPermissions = await _context.RolePermissions
                        .Where(rp => rp.RoleId == adminRole.Id)
                        .ToListAsync();

                    if (!existingPermissions.Any())
                    {
                        var allPermissions = new List<string>
                        {
                            SystemPermissions.PROG01, SystemPermissions.PROG02, SystemPermissions.PROG03,
                            SystemPermissions.PROG11, SystemPermissions.PROG12, SystemPermissions.PROG13,
                            SystemPermissions.PROG14, SystemPermissions.PROG21, SystemPermissions.PROG22,
                            SystemPermissions.PROG23, SystemPermissions.PROG24, SystemPermissions.PROG25,
                            SystemPermissions.PROG29, SystemPermissions.PROG31, SystemPermissions.PROG32,
                            SystemPermissions.PROG33, SystemPermissions.PROG34, SystemPermissions.PROG35
                        };

                        var rolePermissions = allPermissions.Select(code => new RolePermission
                        {
                            RoleId = adminRole.Id,
                            PermissionCode = code,
                            PermissionName = GetPermissionName(code),
                            Module = GetModuleName(code),
                            IsAllowed = true,
                            CreatedAt = DateTime.UtcNow
                        }).ToList();

                        await _context.RolePermissions.AddRangeAsync(rolePermissions);
                        await _context.SaveChangesAsync();

                        TempData["Success"] = $"تم إضافة {rolePermissions.Count} صلاحية لدور Admin بنجاح";
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

        private string GetPermissionName(string code)
        {
            return code switch
            {
                "Prog01" => "تسجيل حركة الوارد",
                "Prog02" => "تسجيل حركة المنصرف",
                "Prog03" => "تسجيل حركة التحويل",
                "Prog11" => "تسجيل المرتجعات",
                "Prog12" => "استعلام وبحث البيانات",
                "Prog13" => "تسجيل ارصده الفتح",
                "Prog14" => "الأرصدة الحالية",
                "Prog21" => "كارت الصنف",
                "Prog22" => "جرد المخزن",
                "Prog23" => "تقارير الارصده والاستهلاك",
                "Prog24" => "جرد كل المخازن",
                "Prog25" => "أكواد الأصناف",
                "Prog29" => "أكواد الموردين",
                "Prog31" => "ادارات البورصه",
                "Prog32" => "العاملين بالبورصه",
                "Prog33" => "صلاحيات المستخدمين",
                "Prog34" => "تهيئه اعدادات النظام",
                "Prog35" => "ترحيل الحركه اليوميه",
                _ => code
            };
        }

        private string GetModuleName(string code)
        {
            return code switch
            {
                "Prog01" or "Prog02" or "Prog03" or "Prog11" or "Prog12" => "الحركة اليومية",
                "Prog13" or "Prog14" or "Prog21" or "Prog22" or "Prog23" or "Prog24" => "المخزون",
                "Prog25" or "Prog29" or "Prog31" or "Prog32" => "أكواد النظام",
                "Prog33" or "Prog34" or "Prog35" => "الإدارة",
                _ => "أخرى"
            };
        }
    }
}
