using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using InventoryManagement.Models.UserManagement;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<UserManagementController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: UserManagement
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .Select(u => new UserListViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    FullName = u.FullName,
                    EmployeeCode = u.EmployeeCode.HasValue ? u.EmployeeCode.Value.ToString() : string.Empty,
                    Department = u.Department,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt,
                    Roles = _userManager.GetRolesAsync(u).GetAwaiter().GetResult().ToList()
                })
                .ToListAsync();

            return View(users);
        }

        // GET: UserManagement/Roles
        public async Task<IActionResult> Roles()
        {
            var roles = await _roleManager.Roles
                .Select(r => new RoleListViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Level = r.Level,
                    IsSystemRole = r.IsSystemRole,
                    CreatedAt = r.CreatedAt,
                    UserCount = _userManager.GetUsersInRoleAsync(r.Name).GetAwaiter().GetResult().Count
                })
                .ToListAsync();

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
                return RedirectToAction(nameof(AssignRoles));
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

        // GET: UserManagement/CreateRole
        public IActionResult CreateRole()
        {
            return View(new CreateRoleViewModel());
        }

        // POST: UserManagement/CreateRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
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
                    TempData["Success"] = "Role created successfully.";
                    return RedirectToAction(nameof(Roles));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        // GET: UserManagement/EditRole/5
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            if (role.IsSystemRole)
            {
                TempData["Error"] = "Cannot edit system roles.";
                return RedirectToAction(nameof(Roles));
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Level = role.Level,
                IsSystemRole = role.IsSystemRole
            };

            return View(model);
        }

        // POST: UserManagement/EditRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    return NotFound();
                }

                if (role.IsSystemRole)
                {
                    TempData["Error"] = "Cannot edit system roles.";
                    return RedirectToAction(nameof(Roles));
                }

                role.Name = model.Name;
                role.Description = model.Description;
                role.Level = model.Level;
                role.UpdatedAt = DateTime.UtcNow;

                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Role updated successfully.";
                    return RedirectToAction(nameof(Roles));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        // POST: UserManagement/DeleteRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            if (role.IsSystemRole)
            {
                TempData["Error"] = "Cannot delete system roles.";
                return RedirectToAction(nameof(Roles));
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Any())
            {
                TempData["Error"] = "Cannot delete role with assigned users.";
                return RedirectToAction(nameof(Roles));
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["Success"] = "Role deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete role.";
            }

            return RedirectToAction(nameof(Roles));
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

            return RedirectToAction(nameof(Index));
        }

        // POST: UserManagement/Delete/5
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

            return RedirectToAction(nameof(Index));
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
    }
}
