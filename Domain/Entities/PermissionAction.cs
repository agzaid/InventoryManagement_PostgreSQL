using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class PermissionAction
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
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}
