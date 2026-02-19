using System;

namespace Application.Interfaces.Models
{
    public class LowStockItemDto
    {
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public string CategoryName { get; set; }
        public decimal OpenBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal? ReorderQuantity { get; set; }
        public DateTime? LastMovementDate { get; set; }
    }
}
