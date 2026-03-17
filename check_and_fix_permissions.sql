-- Check Admin role status
SELECT "Id", "Name", "IsActive" FROM "AspNetRoles" WHERE "Name" = 'Admin';

-- Check if Admin role has any permissions
SELECT COUNT(*) as PermissionCount FROM "RolePermissions" 
WHERE "RoleId" = (SELECT "Id" FROM "AspNetRoles" WHERE "Name" = 'Admin');

-- Check what permissions exist for Admin
SELECT * FROM "RolePermissions" 
WHERE "RoleId" = (SELECT "Id" FROM "AspNetRoles" WHERE "Name" = 'Admin');

-- If Admin role is inactive, activate it
UPDATE "AspNetRoles" SET "IsActive" = true WHERE "Name" = 'Admin';

-- If no permissions exist, insert them (run this if count is 0)
INSERT INTO "RolePermissions" ("RoleId", "PermissionCode", "PermissionName", "Module", "IsAllowed", "CreatedAt")
SELECT 
    r."Id",
    p.code,
    p.name,
    p.module,
    true,
    NOW()
FROM "AspNetRoles" r
CROSS JOIN (
    VALUES 
        ('Prog01', 'تسجيل حركة الوارد', 'الحركة اليومية'),
        ('Prog02', 'تسجيل حركة المنصرف', 'الحركة اليومية'),
        ('Prog03', 'تسجيل حركة التحويل', 'الحركة اليومية'),
        ('Prog11', 'تسجيل المرتجعات', 'الحركة اليومية'),
        ('Prog12', 'استعلام وبحث البيانات', 'الحركة اليومية'),
        ('Prog13', 'تسجيل ارصده الفتح', 'المخزون'),
        ('Prog14', 'الأرصدة الحالية', 'المخزون'),
        ('Prog21', 'كارت الصنف', 'المخزون'),
        ('Prog22', 'جرد المخزن', 'المخزون'),
        ('Prog23', 'تقارير الارصده والاستهلاك', 'المخزون'),
        ('Prog24', 'جرد كل المخازن', 'المخزون'),
        ('Prog25', 'أكواد الأصناف', 'أكواد النظام'),
        ('Prog29', 'أكواد الموردين', 'أكواد النظام'),
        ('Prog31', 'ادارات البورصه', 'أكواد النظام'),
        ('Prog32', 'العاملين بالبورصه', 'أكواد النظام'),
        ('Prog33', 'صلاحيات المستخدمين', 'الإدارة'),
        ('Prog34', 'تهيئه اعدادات النظام', 'الإدارة'),
        ('Prog35', 'ترحيل الحركه اليوميه', 'الإدارة')
) AS p(code, name, module)
WHERE r."Name" = 'Admin'
AND NOT EXISTS (
    SELECT 1 FROM "RolePermissions" rp 
    WHERE rp."RoleId" = r."Id" AND rp."PermissionCode" = p.code
);

-- Verify the insert
SELECT COUNT(*) as TotalPermissions FROM "RolePermissions" 
WHERE "RoleId" = (SELECT "Id" FROM "AspNetRoles" WHERE "Name" = 'Admin');
