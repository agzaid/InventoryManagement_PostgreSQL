using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Persistance;

namespace InventoryManagement.Services
{
    public class RoleSeedingService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RoleSeedingService> _logger;
        private readonly InventoryManagementDbContext _context;

        public RoleSeedingService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<RoleSeedingService> logger, InventoryManagementDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public async Task SeedRolesAsync()
        {
            var roles = new List<ApplicationRole>
            {
                new ApplicationRole
                {
                    Name = "Admin",
                    Description = "System Administrator with full access",
                    Level = 100,
                    IsSystemRole = true,
                    NormalizedName = "ADMIN"
                },
                new ApplicationRole
                {
                    Name = "Manager",
                    Description = "Department Manager with limited admin access",
                    Level = 50,
                    IsSystemRole = true,
                    NormalizedName = "MANAGER"
                },
                new ApplicationRole
                {
                    Name = "User",
                    Description = "Regular user with basic access",
                    Level = 10,
                    IsSystemRole = true,
                    NormalizedName = "USER"
                }
            };

            foreach (var role in roles)
            {
                var existingRole = await _roleManager.FindByNameAsync(role.Name);
                if (existingRole == null)
                {
                    var result = await _roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Role {RoleName} created successfully", role.Name);
                        
                        // Add permissions to roles
                        await AddRolePermissionsAsync(role);
                    }
                    else
                    {
                        _logger.LogError("Failed to create role {RoleName}: {Errors}", 
                            role.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

        private async Task AddRolePermissionsAsync(ApplicationRole role)
        {
            var permissions = new Dictionary<string, List<string>>
            {
                ["Admin"] = new List<string>
                {
                    SystemPermissions.PROG01, SystemPermissions.PROG02, SystemPermissions.PROG03,
                    SystemPermissions.PROG11, SystemPermissions.PROG12, SystemPermissions.PROG13,
                    SystemPermissions.PROG14, SystemPermissions.PROG21, SystemPermissions.PROG22,
                    SystemPermissions.PROG23, SystemPermissions.PROG24, SystemPermissions.PROG25,
                    SystemPermissions.PROG29, SystemPermissions.PROG31, SystemPermissions.PROG32,
                    SystemPermissions.PROG33, SystemPermissions.PROG34, SystemPermissions.PROG35
                },
                ["Manager"] = new List<string>
                {
                    SystemPermissions.PROG01, SystemPermissions.PROG02, SystemPermissions.PROG03,
                    SystemPermissions.PROG11, SystemPermissions.PROG12, SystemPermissions.PROG13,
                    SystemPermissions.PROG14, SystemPermissions.PROG21, SystemPermissions.PROG22,
                    SystemPermissions.PROG23, SystemPermissions.PROG24, SystemPermissions.PROG25,
                    SystemPermissions.PROG29
                },
                ["User"] = new List<string>
                {
                    SystemPermissions.PROG01, SystemPermissions.PROG02, SystemPermissions.PROG03,
                    SystemPermissions.PROG11, SystemPermissions.PROG12, SystemPermissions.PROG13,
                    SystemPermissions.PROG14
                }
            };

            if (permissions.ContainsKey(role.Name))
            {
                var rolePermissions = permissions[role.Name].Select(permissionCode => new RolePermission
                {
                    RoleId = role.Id,
                    PermissionCode = permissionCode,
                    PermissionName = GetPermissionDisplayName(permissionCode),
                    Module = GetPermissionModule(permissionCode),
                    IsAllowed = true,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                // Save permissions to database
                await _context.RolePermissions.AddRangeAsync(rolePermissions);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Added {Count} permissions to role {RoleName}", 
                    rolePermissions.Count, role.Name);
            }
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

        private string GetPermissionModule(string code)
        {
            return code switch
            {
                "PROG01" or "PROG02" or "PROG03" or "PROG11" or "PROG12" => "Daily Movement",
                "PROG13" or "PROG14" or "PROG21" or "PROG22" or "PROG23" or "PROG24" => "Inventory",
                "PROG25" or "PROG29" or "PROG31" or "PROG32" => "System Codes",
                "PROG33" or "PROG34" or "PROG35" => "Admin",
                _ => "Other"
            };
        }

        public async Task SeedAdminUserAsync()
        {
            try
            {
                // Check if admin user already exists
                var adminUser = await _userManager.FindByEmailAsync("admin@gmail.com");
                if (adminUser != null)
                {
                    _logger.LogInformation("Admin user already exists.");
                    return;
                }

                // Create admin user
                var user = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Administrator",
                    FullName = "System Administrator",
                    EmployeeCode = 0,
                    Department = "IT",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, "Admin@123");

                if (result.Succeeded)
                {
                    // Add admin role to user
                    await _userManager.AddToRoleAsync(user, "Admin");
                    _logger.LogInformation("Admin user created successfully with email: admin@gmail.com");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError($"Failed to create admin user: {errors}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding admin user.");
            }
        }
    }
}
