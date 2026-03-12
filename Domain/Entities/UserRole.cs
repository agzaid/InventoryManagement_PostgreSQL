namespace Domain.Entities
{
    public class UserRole
    {
        public string UserId { get; set; } = string.Empty; // Changed to string for ASP.NET Identity
        public string RoleId { get; set; } = string.Empty; // Changed to string for ASP.NET Identity
        
        // Navigation properties
        public virtual ApplicationRole Role { get; set; } = null!;
        // Note: User navigation removed since we're using ApplicationUser from ASP.NET Identity
        // public virtual User User { get; set; } = null!;
    }
}
