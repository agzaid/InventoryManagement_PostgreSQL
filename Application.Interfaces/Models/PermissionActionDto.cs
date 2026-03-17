namespace Application.Interfaces.Models
{
    public class PermissionActionDto
    {
        public int Id { get; set; }
        public string PermissionCode { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? FullUrl { get; set; }
        public string? HttpMethod { get; set; } = "GET";
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class PermissionActionManagementDto
    {
        public List<PermissionActionDto> PermissionActions { get; set; } = new();
        public List<PermissionModuleDto> AvailablePermissions { get; set; } = new();
        public List<ControllerActionDto> AvailableControllers { get; set; } = new();
    }

    public class PermissionModuleDto
    {
        public string Module { get; set; } = string.Empty;
        public string ModuleNameArabic { get; set; } = string.Empty;
        public List<PermissionItemDto> Permissions { get; set; } = new();
    }

    public class PermissionItemDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsAssigned { get; set; }
    }

    public class ControllerActionDto
    {
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = "GET";
    }
}
