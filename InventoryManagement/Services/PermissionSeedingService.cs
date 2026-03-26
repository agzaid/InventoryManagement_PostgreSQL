using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace InventoryManagement.Services
{
    public class PermissionSeedingService
    {
        private readonly InventoryManagementDbContext _context;
        private readonly ILogger<PermissionSeedingService> _logger;

        public PermissionSeedingService(
            InventoryManagementDbContext context,
            ILogger<PermissionSeedingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> SeedPermissionsAsync()
        {
            try
            {
                var permissions = GetAllPermissionDefinitions();
                int seededCount = 0;

                // Get all existing permission codes to avoid duplicates
                var existingCodes = await _context.RolePermissions
                    .Select(rp => rp.PermissionCode)
                    .Distinct()
                    .ToListAsync();

                // For each permission definition, ensure at least one record exists in the database
                // This allows the GetAllPermissions API to query distinct permissions
                foreach (var permission in permissions)
                {
                    if (!existingCodes.Contains(permission.Code))
                    {
                        // Create a placeholder record - this will be used as reference data
                        // Actual role assignments will be separate records
                        _context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = null, // Null indicates system-defined permission template
                            PermissionCode = permission.Code,
                            PermissionName = permission.Name,
                            Module = permission.Module,
                            IsAllowed = false,
                            CreatedAt = DateTime.UtcNow
                        });
                        seededCount++;
                    }
                }

                if (seededCount > 0)
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Seeded {seededCount} new permissions");
                }
                else
                {
                    _logger.LogInformation("All permissions already exist in database");
                }

                return seededCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding permissions");
                throw;
            }
        }

        private List<PermissionDefinition> GetAllPermissionDefinitions()
        {
            return new List<PermissionDefinition>
            {
                // Daily Movement Module
                new PermissionDefinition { Code = "PROG01", Name = "تسجيل حركة الوارد", Module = "الحركة اليومية" },
                new PermissionDefinition { Code = "PROG02", Name = "تسجيل حركة المنصرف", Module = "الحركة اليومية" },
                new PermissionDefinition { Code = "PROG03", Name = "تسجيل حركة التحويل", Module = "الحركة اليومية" },
                new PermissionDefinition { Code = "PROG11", Name = "تسجيل المرتجعات", Module = "الحركة اليومية" },
                new PermissionDefinition { Code = "PROG12", Name = "استعلام وبحث البيانات", Module = "الحركة اليومية" },

                // Inventory Module
                new PermissionDefinition { Code = "PROG13", Name = "تسجيل أرصدة الفتح", Module = "إدارة المخزون" },
                new PermissionDefinition { Code = "PROG14", Name = "الأرصدة الحالية", Module = "إدارة المخزون" },
                new PermissionDefinition { Code = "PROG21", Name = "كارت الصنف", Module = "إدارة المخزون" },
                new PermissionDefinition { Code = "PROG22", Name = "جرد المخزن", Module = "إدارة المخزون" },
                new PermissionDefinition { Code = "PROG23", Name = "تقارير الأرصدة والاستهلاك", Module = "إدارة المخزون" },
                new PermissionDefinition { Code = "PROG24", Name = "جرد كل المخازن", Module = "إدارة المخزون" },

                // System Codes Module
                new PermissionDefinition { Code = "PROG25", Name = "أكواد الأصناف", Module = "أكواد النظام" },
                new PermissionDefinition { Code = "PROG29", Name = "أكواد الموردين", Module = "أكواد النظام" },
                new PermissionDefinition { Code = "PROG31", Name = "إدارات البورصة", Module = "أكواد النظام" },
                new PermissionDefinition { Code = "PROG32", Name = "العاملين بالبورصة", Module = "أكواد النظام" },

                // Admin Module
                new PermissionDefinition { Code = "PROG33", Name = "صلاحيات المستخدمين", Module = "إدارة النظام" },
                new PermissionDefinition { Code = "PROG34", Name = "تهيئة إعدادات النظام", Module = "إدارة النظام" },
                new PermissionDefinition { Code = "PROG35", Name = "ترحيل الحركة اليومية", Module = "إدارة النظام" },
                new PermissionDefinition { Code = "PROG99", Name = "لا يوجد", Module = "إدارة النظام" },
            };
        }

        private class PermissionDefinition
        {
            public string Code { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Module { get; set; } = string.Empty;
        }
    }
}
