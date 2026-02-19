using System;

namespace Application.Interfaces.Models
{
    public class RecentActivityDto
    {
        public int TrNum { get; set; }
        public int TrType { get; set; }
        public string TrTypeName { get; set; }
        public DateTime TrDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public decimal ItemQnt { get; set; }
        public string Party { get; set; }
    }
}
