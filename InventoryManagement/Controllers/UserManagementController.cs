using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using InventoryManagement.Models.UserManagement;
using Application.Interfaces.Contracts.Service;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class UserManagementController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<UserManagementController> _logger;
        private readonly ISystemManagementService _systemManagementService;
        private readonly IRoleManagementService _roleManagementService;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<UserManagementController> logger,
            ISystemManagementService systemManagementService,
            IRoleManagementService roleManagementService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _systemManagementService = systemManagementService;
            _roleManagementService = roleManagementService;
        }

        // GET: UserManagement
        public async Task<IActionResult> Index()
        {
            var model = await _systemManagementService.GetAllEgxEmployeesAsync();
            return View(model);
        }

        // GET: UserManagement/GetRolesWithPermissions
        [HttpGet]
        public async Task<IActionResult> GetRolesWithPermissions()
        {
            var roles = await _roleManager.Roles
                .Where(r => r.IsActive)
                .Include(r => r.RolePermissions)
                .Select(r => new
                {
                    id = r.Id,
                    name = r.Name,
                    description = r.Description,
                    permissions = r.RolePermissions
                        .Where(rp => rp.IsAllowed)
                        .Select(rp => rp.PermissionCode)
                        .ToList()
                })
                .ToListAsync();

            return Json(roles);
        }

        // GET: UserManagement/GetAllPermissions
        [HttpGet]
        public async Task<IActionResult> GetAllPermissions()
        {
            // Get all distinct permissions from service
            var allPermissions = await _roleManagementService.GetAllDistinctPermissionsAsync();
            
            var grouped = allPermissions
                .GroupBy(rp => rp.Module)
                .Select(g => new
                {
                    module = g.Key,
                    permissions = g.Select(p => new
                    {
                        code = p.PermissionCode,
                        name = p.PermissionName
                    }).ToList()
                })
                .ToList();

            return Json(grouped);
        }

        // GET: UserManagement/GetModuleColors
        [HttpGet]
        public IActionResult GetModuleColors()
        {
            var moduleColors = _roleManagementService.GetModuleColors();
            return Json(moduleColors);
        }

        // GET: UserManagement/Roles
        public async Task<IActionResult> Roles()
        {
            var roleList = await _roleManager.Roles.ToListAsync();
            
            var roles = new List<RoleListViewModel>();
            foreach (var r in roleList)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(r.Name);
                roles.Add(new RoleListViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Level = r.Level,
                    IsSystemRole = r.IsSystemRole,
                    CreatedAt = r.CreatedAt,
                    UserCount = usersInRole.Count
                });
            }

            return View(roles);
        }

        // GET: UserManagement/ManageRoles
        public async Task<IActionResult> ManageRoles()
        {
            var roleList = await _roleManager.Roles.ToListAsync();
            
            var roles = new List<RoleListViewModel>();
            foreach (var r in roleList)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(r.Name);
                roles.Add(new RoleListViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Level = r.Level,
                    IsSystemRole = r.IsSystemRole,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    UserCount = usersInRole.Count
                });
            }

            // Get all permissions grouped by module from service
            var systemPermissions = await _roleManagementService.GetSystemPermissionsAsync();
            var permissionsGrouped = systemPermissions
                .GroupBy(rp => rp.Module)
                .Select(g => new
                {
                    Module = g.Key,
                    Permissions = g.Select(p => p.PermissionName).ToList()
                })
                .ToList();

            ViewBag.PermissionsGrouped = permissionsGrouped;

            return View(roles);
        }

        // GET: UserManagement/AssignRoles
        public async Task<IActionResult> AssignRoles()
        {
            var users = await _userManager.Users
                .Select(u => new UserSelectionViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    EmployeeCode = u.EmployeeCode.HasValue ? u.EmployeeCode.Value.ToString() : string.Empty
                })
                .ToListAsync();

            var roles = await _roleManager.Roles
                .Select(r => new RoleSelectionViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToListAsync();

            var model = new AssignRolesViewModel
            {
                Users = users,
                Roles = roles,
                UserRoles = new List<UserRoleAssignmentViewModel>()
            };

            // Load existing role assignments
            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.Id));
                foreach (var role in userRoles)
                {
                    model.UserRoles.Add(new UserRoleAssignmentViewModel
                    {
                        UserId = user.Id,
                        RoleId = roles.FirstOrDefault(r => r.Name == role)?.Id ?? "",
                        RoleName = role
                    });
                }
            }

            return View(model);
        }

        // POST: UserManagement/AssignRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRoles(AssignRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var userRole in model.UserRoles)
                {
                    var user = await _userManager.FindByIdAsync(userRole.UserId);
                    if (user != null)
                    {
                        var currentRoles = await _userManager.GetRolesAsync(user);
                        
                        // Remove all current roles
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        
                        // Add new roles
                        if (!string.IsNullOrEmpty(userRole.RoleId))
                        {
                            var role = await _roleManager.FindByIdAsync(userRole.RoleId);
                            if (role != null)
                            {
                                await _userManager.AddToRoleAsync(user, role.Name);
                            }
                        }
                    }
                }

                TempData["Success"] = "Role assignments updated successfully.";
                return RedirectToActionWithCulture(nameof(AssignRoles));
            }

            // Reload data if validation fails
            await PopulateAssignRolesModel(model);
            return View(model);
        }

        // GET: UserManagement/UserPermissions/5
        public async Task<IActionResult> UserPermissions(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserPermissionViewModel
            {
                UserId = userId,
                UserName = user.UserName,
                FullName = user.FullName,
                Permissions = new List<PermissionItemViewModel>()
            };

            // Get all available permissions
            var allPermissions = typeof(SystemPermissions)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.FieldType == typeof(string))
                .Select(f => new
                {
                    Code = f.GetValue(null)?.ToString(),
                    Name = GetPermissionDisplayName(f.Name)
                })
                .ToList();

            foreach (var permission in allPermissions)
            {
                model.Permissions.Add(new PermissionItemViewModel
                {
                    Code = permission.Code ?? "",
                    Name = permission.Name,
                    IsGranted = false // TODO: Check actual user permissions
                });
            }

            return View(model);
        }

        #region Role Management

        // GET: UserManagement/GetRolePermissions
        [HttpGet]
        public async Task<IActionResult> GetRolePermissions(string roleId)
        {
            // Get only the permissions assigned to this specific role from service
            var permissions = await _roleManagementService.GetRolePermissionsAsync(roleId);
            
            var result = permissions.Select(rp => new
            {
                permissionCode = rp.PermissionCode,
                permissionName = rp.PermissionName,
                module = rp.Module
            });

            return Json(result);
        }

        // GET: UserManagement/CreateRole
        public IActionResult CreateRole()
        {
            return View(new CreateRoleViewModel());
        }

        // POST: UserManagement/CreateRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model, List<string> selectedPermissions)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole
                {
                    Name = model.Name,
                    Description = model.Description,
                    Level = model.Level,
                    IsSystemRole = model.IsSystemRole,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    // Add permissions to role using service
                    if (selectedPermissions != null && selectedPermissions.Any())
                    {
                        await _roleManagementService.UpdateRolePermissionsAsync(role.Id, selectedPermissions);
                    }

                    TempData["Success"] = "تم إنشاء الدور بنجاح";
                    return RedirectToActionWithCulture(nameof(ManageRoles));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            TempData["CreateRoleErrors"] = string.Join("; ", errors);
            TempData["CreateRoleModel"] = System.Text.Json.JsonSerializer.Serialize(model);
            return RedirectToActionWithCulture(nameof(ManageRoles));
        }

        // GET: UserManagement/EditRole/5
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["Error"] = "الدور غير موجود";
                return RedirectToActionWithCulture(nameof(ManageRoles));
            }

            // Get role's assigned permissions from service
            var rolePermissions = await _roleManagementService.GetRolePermissionsAsync(id);
            var permissions = rolePermissions.Select(rp => rp.PermissionCode).ToList();

            // Get all available permissions grouped by module from service
            var systemPermissions = await _roleManagementService.GetSystemPermissionsAsync();
            var permissionsGrouped = systemPermissions
                .GroupBy(rp => rp.Module)
                .Select(g => new
                {
                    Module = g.Key,
                    Permissions = g.Select(p => new { p.PermissionCode, p.PermissionName }).ToList()
                })
                .ToList();

            ViewBag.PermissionsGrouped = permissionsGrouped;

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Level = role.Level,
                IsSystemRole = role.IsSystemRole,
                SelectedPermissions = permissions
            };

            return View(model);
        }

        // POST: UserManagement/EditRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(EditRoleViewModel model, List<string> selectedPermissions)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    TempData["Error"] = "الدور غير موجود";
                    return RedirectToActionWithCulture(nameof(ManageRoles));
                }

                role.Name = model.Name;
                role.Description = model.Description;
                role.Level = model.Level;
                role.UpdatedAt = DateTime.UtcNow;

                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    // Update permissions using service
                    await _roleManagementService.UpdateRolePermissionsAsync(role.Id, selectedPermissions ?? new List<string>());
                    
                    TempData["Success"] = "تم تحديث الدور بنجاح";
                    return RedirectToActionWithCulture(nameof(ManageRoles));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        // POST: UserManagement/ToggleRoleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleRoleStatus(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["Error"] = "الدور غير موجود";
                return RedirectToActionWithCulture(nameof(ManageRoles));
            }

            role.IsActive = !role.IsActive;
            role.UpdatedAt = DateTime.UtcNow;

            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                var status = role.IsActive ? "تفعيل" : "تعطيل";
                TempData["Success"] = $"تم {status} الدور '{role.Name}' بنجاح";
            }
            else
            {
                TempData["Error"] = "فشل تحديث حالة الدور";
            }

            return RedirectToActionWithCulture(nameof(ManageRoles));
        }

        // POST: UserManagement/DeleteRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["Error"] = "الدور غير موجود";
                return RedirectToActionWithCulture(nameof(ManageRoles));
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Any())
            {
                TempData["Error"] = $"لا يمكن حذف الدور '{role.Name}' - يوجد {usersInRole.Count} مستخدم مرتبط بهذا الدور. يجب إزالة جميع المستخدمين من هذا الدور أولاً";
                return RedirectToActionWithCulture(nameof(ManageRoles));
            }

            // Delete associated permissions first using service
            await _roleManagementService.DeleteRolePermissionsAsync(id);

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["Success"] = $"تم حذف الدور '{role.Name}' بنجاح";
            }
            else
            {
                TempData["Error"] = "فشل حذف الدور";
            }

            return RedirectToActionWithCulture(nameof(ManageRoles));
        }

        #endregion

        #region User Management

        // GET: UserManagement/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new InventoryManagement.Models.UserManagement.ProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                FullName = user.FullName,
                EmployeeCode = user.EmployeeCode?.ToString() ?? string.Empty,
                Department = user.Department,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Roles = roles.ToList()
            };

            return View(model);
        }

        // POST: UserManagement/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = $"User {(user.IsActive ? "activated" : "deactivated")} successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to update user status.";
            }

            return RedirectToActionWithCulture(nameof(Index));
        }

        // POST: UserManagement/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(Application.Interfaces.Models.InvUserDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _systemManagementService.CreateInvUserAsync(model);
                    TempData["Success"] = "تم إنشاء المستخدم بنجاح";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"فشل في إنشاء المستخدم: {ex.Message}";
                }
            }
            else
            {
                TempData["Error"] = "البيانات المدخلة غير صحيحة";
            }

            return RedirectToActionWithCulture(nameof(Index));
        }

        // GET: UserManagement/EditUser/5
        public async Task<IActionResult> EditUser(int id)
        {
            var model = await _systemManagementService.GetAllEgxEmployeesAsync();
            var user = model.invUserDto?.FirstOrDefault(u => u.Id == id);
            
            if (user == null)
            {
                TempData["Error"] = "المستخدم غير موجود";
                return RedirectToActionWithCulture(nameof(Index));
            }

            ViewBag.Employees = model.EgxEmployeeDto;
            return View(user);
        }

        // POST: UserManagement/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(Application.Interfaces.Models.InvUserDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _systemManagementService.UpdateInvUserAsync(model);
                    TempData["Success"] = "تم تحديث المستخدم بنجاح";
                    return RedirectToActionWithCulture(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"فشل في تحديث المستخدم: {ex.Message}";
                }
            }
            else
            {
                TempData["Error"] = "البيانات المدخلة غير صحيحة";
            }

            var data = await _systemManagementService.GetAllEgxEmployeesAsync();
            ViewBag.Employees = data.EgxEmployeeDto;
            return View(model);
        }

        // GET: UserManagement/ManageUserPermissions/5
        public async Task<IActionResult> ManageUserPermissions(int id)
        {
            var model = await _systemManagementService.GetAllEgxEmployeesAsync();
            var user = model.invUserDto?.FirstOrDefault(u => u.Id == id);
            
            if (user == null)
            {
                TempData["Error"] = "المستخدم غير موجود";
                return RedirectToActionWithCulture(nameof(Index));
            }

            return View(user);
        }

        // POST: UserManagement/ManageUserPermissions/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserPermissions(Application.Interfaces.Models.InvUserDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _systemManagementService.UpdateInvUserAsync(model);
                    TempData["Success"] = "تم تحديث صلاحيات المستخدم بنجاح";
                    return RedirectToActionWithCulture(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"فشل في تحديث الصلاحيات: {ex.Message}";
                }
            }

            return View(model);
        }

        // POST: UserManagement/DeleteUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _systemManagementService.DeleteInvUserAsync(id);
                TempData["Success"] = "تم حذف المستخدم بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"فشل في حذف المستخدم: {ex.Message}";
            }

            return RedirectToActionWithCulture(nameof(Index));
        }

        // POST: UserManagement/Delete/5 (Keep for backward compatibility)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "User deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete user.";
            }

            return RedirectToActionWithCulture(nameof(Index));
        }

        #endregion

        private async Task PopulateAssignRolesModel(AssignRolesViewModel model)
        {
            model.Users = await _userManager.Users
                .Select(u => new UserSelectionViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    EmployeeCode = u.EmployeeCode.HasValue ? u.EmployeeCode.Value.ToString() : string.Empty
                })
                .ToListAsync();

            model.Roles = await _roleManager.Roles
                .Select(r => new RoleSelectionViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToListAsync();
        }

        private string GetPermissionDisplayName(string code)
        {
            return code switch
            {
                "PROG01" => "تسجيل حركة الوارد",
                "PROG02" => "تسجيل حركة المنصرف",
                "PROG03" => "تسجيل حركة التحويل",
                "PROG11" => "تسجيل المرتجعات",
                "PROG12" => "استعلام وبحث البيانات",
                "PROG13" => "تسجيل ارصده الفتح",
                "PROG14" => "الأرصدة الحالية",
                "PROG21" => "كارت الصنف",
                "PROG22" => "جرد المخزن",
                "PROG23" => "تقارير الارصده والاستهلاك",
                "PROG24" => "جرد كل المخازن",
                "PROG25" => "أكواد الأصناف",
                "PROG29" => "أكواد الموردين",
                "PROG31" => "ادارات البورصه",
                "PROG32" => "العاملين بالبورصه",
                "PROG33" => "صلاحيات المستخدمين",
                "PROG34" => "تهيئه اعدادات النظام",
                "PROG35" => "ترحيل الحركه اليوميه",
                _ => code
            };
        }


        // Quick action to seed Admin role permissions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SeedAdminPermissions()
        {
            try
            {
                var adminRole = await _roleManager.FindByNameAsync("Admin");
                if (adminRole == null)
                {
                    TempData["Error"] = "دور Admin غير موجود";
                    return RedirectToActionWithCulture(nameof(ManageRoles));
                }

                // Check if permissions already exist using service
                var existingCount = await _roleManagementService.GetRolePermissionCountAsync(adminRole.Id);

                if (existingCount > 0)
                {
                    TempData["Info"] = $"دور Admin يحتوي بالفعل على {existingCount} صلاحية";
                    return RedirectToActionWithCulture(nameof(ManageRoles));
                }

                // Seed all permissions to Admin role using service
                await _roleManagementService.SeedAllPermissionsToAdminAsync(adminRole.Id);
                
                var allPermissionsCount = (await _roleManagementService.GetSystemPermissionsAsync()).Count;
                TempData["Success"] = $"تم إضافة {allPermissionsCount} صلاحية لدور Admin بنجاح. يرجى تسجيل الخروج والدخول مرة أخرى لتفعيل الصلاحيات.";
                return RedirectToActionWithCulture(nameof(ManageRoles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding admin permissions");
                TempData["Error"] = "حدث خطأ أثناء إضافة الصلاحيات: " + ex.Message;
                return RedirectToActionWithCulture(nameof(ManageRoles));
            }
        }

        // GET: UserManagement/ManageApplicationUsers
        public async Task<IActionResult> ManageApplicationUsers()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            var userViewModels = new List<ApplicationUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new ApplicationUserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        // POST: UserManagement/DeleteApplicationUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteApplicationUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "المستخدم غير موجود";
                    return RedirectToActionWithCulture(nameof(ManageApplicationUsers));
                }

                // Prevent deleting the current user
                if (user.UserName == User.Identity.Name)
                {
                    TempData["Error"] = "لا يمكنك حذف حسابك الخاص";
                    return RedirectToActionWithCulture(nameof(ManageApplicationUsers));
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = $"تم حذف المستخدم {user.UserName} بنجاح";
                }
                else
                {
                    TempData["Error"] = "فشل في حذف المستخدم: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting application user");
                TempData["Error"] = $"حدث خطأ أثناء حذف المستخدم: {ex.Message}";
            }

            return RedirectToActionWithCulture(nameof(ManageApplicationUsers));
        }
    }
}
