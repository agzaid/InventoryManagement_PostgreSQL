using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public int? EmployeeCode { get; set; }
        public string? Department { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Note: Address and UpdatedAt will be added later via migration
        // public string? Address { get; set; }
        // public DateTime? UpdatedAt { get; set; }

        // Removed UserRoleAssignment navigation - using ASP.NET Identity's built-in user roles
        // public virtual ICollection<UserRoleAssignment> RoleAssignments { get; set; } = new List<UserRoleAssignment>();
    }

    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
        public int Level { get; set; } = 0; // Higher number = higher privilege
        public bool IsSystemRole { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties for role permissions
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        
        // Navigation properties for custom UserRole assignments
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    // Removed UserRoleAssignment - using ASP.NET Identity's AspNetUserRoles instead
    // public class UserRoleAssignment
    // {
    //     public int Id { get; set; }
    //     public string UserId { get; set; } = string.Empty;
    //     public string RoleId { get; set; } = string.Empty;
    //     public string PermissionCode { get; set; } = string.Empty;
    //     public bool IsGranted { get; set; } = true;
    //     public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    //     public string? AssignedBy { get; set; }
    //
    //     public virtual ApplicationUser User { get; set; } = null!;
    //     public virtual ApplicationRole Role { get; set; } = null!;
    // }

    public class RolePermission
    {
        public int Id { get; set; }
        public string? RoleId { get; set; }
        public string PermissionCode { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public bool IsAllowed { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ApplicationRole? Role { get; set; }
    }

    // Permission constants matching your existing system
    public static class SystemPermissions
    {
        // Daily Movement Module
        public const string PROG01 = "Prog01"; // تسجيل حركة الوارد
        public const string PROG02 = "Prog02"; // تسجيل حركة المنصرف
        public const string PROG03 = "Prog03"; // تسجيل حركة التحويل
        public const string PROG11 = "Prog11"; // تسجيل المرتجعات
        public const string PROG12 = "Prog12"; // استعلام وبحث البيانات

        // Inventory Module
        public const string PROG13 = "Prog13"; // تسجيل ارصده الفتح
        public const string PROG14 = "Prog14"; // الأرصدة الحالية
        public const string PROG21 = "Prog21"; // كارت الصنف
        public const string PROG22 = "Prog22"; // جرد المخزن
        public const string PROG23 = "Prog23"; // تقارير الارصده والاستهلاك
        public const string PROG24 = "Prog24"; // جرد كل المخازن

        // System Codes Module
        public const string PROG25 = "Prog25"; // أكواد الأصناف
        public const string PROG29 = "Prog29"; // أكواد الموردين
        public const string PROG31 = "Prog31"; // ادارات البورصه
        public const string PROG32 = "Prog32"; // العاملين بالبورصه

        // Admin Module
        public const string PROG33 = "Prog33"; // صلاحيات المستخدمين
        public const string PROG34 = "Prog34"; // تهيئه اعدادات النظام
        public const string PROG35 = "Prog35"; // ترحيل الحركه اليوميه
        public const string PROG99 = "Prog99"; // لا يوجد
    }
}
