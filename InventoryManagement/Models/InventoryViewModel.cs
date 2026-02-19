using System;

namespace InventoryManagement.Models
{
    public class InventoryViewModel
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; }
        public DateTime LastAuditDate { get; set; }
    }
}
