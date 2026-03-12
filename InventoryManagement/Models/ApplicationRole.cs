using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class ApplicationRole : IdentityRole
    {
        [Display(Name = "Role Description")]
        public string? Description { get; set; }

        [Display(Name = "Role Level")]
        public int Level { get; set; } = 0; // Higher number = higher privilege

        [Display(Name = "Is System Role")]
        public bool IsSystemRole { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties for role permissions
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }

    public class RolePermission
    {
        public int Id { get; set; }
        public string RoleId { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public bool IsAllowed { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ApplicationRole Role { get; set; } = null!;
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
    }
}
