using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Domain;

namespace InventoryManagement.Services
{
    public class AdminSeedingService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AdminSeedingService> _logger;

        public AdminSeedingService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<AdminSeedingService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedAdminUserAsync()
        {
            try
            {
                // Create Admin Role if it doesn't exist
                var adminRole = await _roleManager.FindByNameAsync("Admin");
                if (adminRole == null)
                {
                    adminRole = new ApplicationRole
                    {
                        Name = "Admin",
                        NormalizedName = "ADMIN",
                        Description = "مدير النظام",
                        Level = 100,
                        IsSystemRole = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    var roleResult = await _roleManager.CreateAsync(adminRole);
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogError("Failed to create Admin role: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        return;
                    }
                    _logger.LogInformation("Admin role created successfully");
                }

                // Create Admin User if it doesn't exist
                var adminUser = await _userManager.FindByEmailAsync("admin@gmail.com");
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = "admin@gmail.com",
                        Email = "admin@gmail.com",
                        EmailConfirmed = true,
                        FullName = "مدير النظام",
                        EmployeeCode = 1,
                        Department = "الإدارة",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        PhoneNumber = "0000000000"
                    };

                    var userResult = await _userManager.CreateAsync(adminUser, "Admin@123");
                    if (!userResult.Succeeded)
                    {
                        _logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", userResult.Errors.Select(e => e.Description)));
                        return;
                    }
                    _logger.LogInformation("Admin user created successfully");
                }

                // Assign Admin role to user if not already assigned
                if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    var addToRoleResult = await _userManager.AddToRoleAsync(adminUser, "Admin");
                    if (!addToRoleResult.Succeeded)
                    {
                        _logger.LogError("Failed to assign Admin role to user: {Errors}", string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)));
                        return;
                    }
                    _logger.LogInformation("Admin role assigned to user successfully");
                }

                // Seed all permissions for Admin role
                await SeedAdminPermissionsAsync(adminRole.Id);

                _logger.LogInformation("Admin user seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding admin user");
            }
        }

        private async Task SeedAdminPermissionsAsync(string roleId)
        {
            // Get all permission codes from SystemPermissions
            var permissionCodes = typeof(SystemPermissions)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.FieldType == typeof(string))
                .Select(f => f.GetValue(null)?.ToString())
                .Where(code => !string.IsNullOrEmpty(code))
                .ToList();

            _logger.LogInformation("Seeding {Count} permissions for Admin role", permissionCodes.Count);

            // Note: Since we're using ASP.NET Identity's built-in tables,
            // Admin role will have full access by default through the authorization logic
            // The RolePermission table is only needed for custom roles like Storekeeper
            
            _logger.LogInformation("Admin permissions seeding completed");
        }
    }
}
