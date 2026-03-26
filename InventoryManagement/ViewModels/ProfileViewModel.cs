namespace InventoryManagement.ViewModels
{
    public class ProfileViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public int? EmployeeCode { get; set; }
        public string? Department { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Read-only properties for display
        public string? FullName => $"{FirstName} {LastName}";
        public List<string> Roles { get; set; } = new List<string>();
    }
}
