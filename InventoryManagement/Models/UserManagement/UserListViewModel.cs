using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models.UserManagement
{
    public class UserListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Department { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class RoleListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Level { get; set; }
        public bool IsSystemRole { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public int UserCount { get; set; }
    }

    public class UserSelectionViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? EmployeeCode { get; set; }
    }

    public class RoleSelectionViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UserRoleAssignmentViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }

    public class AssignRolesViewModel
    {
        public List<UserSelectionViewModel> Users { get; set; } = new List<UserSelectionViewModel>();
        public List<RoleSelectionViewModel> Roles { get; set; } = new List<RoleSelectionViewModel>();
        public List<UserRoleAssignmentViewModel> UserRoles { get; set; } = new List<UserRoleAssignmentViewModel>();
    }

    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Role name is required")]
        [Display(Name = "Role Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Level")]
        public int Level { get; set; } = 0;

        [Display(Name = "Is System Role")]
        public bool IsSystemRole { get; set; } = false;
    }

    public class EditRoleViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role name is required")]
        [Display(Name = "Role Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Level")]
        public int Level { get; set; } = 0;

        [Display(Name = "Is System Role")]
        public bool IsSystemRole { get; set; } = false;

        public List<string> SelectedPermissions { get; set; } = new List<string>();
    }

    public class UserPermissionViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public List<PermissionItemViewModel> Permissions { get; set; } = new List<PermissionItemViewModel>();
    }

    public class PermissionItemViewModel
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
    }
}
