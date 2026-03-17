using System.ComponentModel.DataAnnotations;

namespace Application.Interfaces.Models
{
    public class PermissionViewModel
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class RolePermissionsViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string? RoleDescription { get; set; }
        public int RoleLevel { get; set; }
        public List<PermissionItemViewModel> Permissions { get; set; } = new List<PermissionItemViewModel>();
    }

    public class PermissionItemViewModel
    {
        public int? Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsAllowed { get; set; }
    }

    public class ManageRolePermissionsViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public List<PermissionGroupViewModel> PermissionGroups { get; set; } = new List<PermissionGroupViewModel>();
    }

    public class PermissionGroupViewModel
    {
        public string ModuleName { get; set; } = string.Empty;
        public List<PermissionItemViewModel> Permissions { get; set; } = new List<PermissionItemViewModel>();
    }

    public class SaveRolePermissionsViewModel
    {
        [Required]
        public string RoleId { get; set; } = string.Empty;
        public List<string> PermissionCodes { get; set; } = new List<string>();
    }

    public class AllPermissionsViewModel
    {
        public List<PermissionGroupViewModel> PermissionGroups { get; set; } = new List<PermissionGroupViewModel>();
        public List<RolePermissionSummaryViewModel> Roles { get; set; } = new List<RolePermissionSummaryViewModel>();
    }

    public class RolePermissionSummaryViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public int PermissionCount { get; set; }
        public int Level { get; set; }
    }
}
