using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Models
{
    public class LowStockNotificationDto
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public string CategoryName { get; set; }
        public decimal CurrentQuantity { get; set; }
        public decimal MinimumQuantity { get; set; }
        public decimal NotificationPercentage { get; set; }
        public bool IsLowStock { get; set; }
        public string StockStatus { get; set; }
        public decimal StockPercentage { get; set; }
    }

    public class UpdateItemThresholdDto
    {
        public int ItemId { get; set; }
        public decimal MinimumQuantity { get; set; }
        public decimal NotificationPercentage { get; set; }
    }

    public class LowStockSummaryDto
    {
        public int TotalItems { get; set; }
        public int LowStockItems { get; set; }
        public int CriticalStockItems { get; set; }
        public decimal LowStockPercentage { get; set; }
    }
}
