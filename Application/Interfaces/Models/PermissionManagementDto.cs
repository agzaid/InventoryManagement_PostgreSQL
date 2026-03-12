namespace Application.Interfaces.Models
{
    public class PermissionDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public bool IsChecked { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }

    public class RolePermissionDto
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = new();
    }

    public class UpdateRolePermissionsRequest
    {
        public string RoleId { get; set; } = string.Empty;
        public List<string> PermissionCodes { get; set; } = new();
    }
}
