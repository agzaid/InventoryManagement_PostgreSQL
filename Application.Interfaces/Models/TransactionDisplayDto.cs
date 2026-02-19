using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Models
{
    public class TransactionDisplayDto
    {
        public string? TrDate2 { get; set; }
        public int? TrNum { get; set; }
        public int? TrSerial { get; set; }
        public int? SupplierCode { get; set; }
        public string? SupplierDesc { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemDesc { get; set; }
        public decimal? ItemQnt { get; set; }
        public decimal? ItemPrice { get; set; }
        public string? BillNum { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliverDate { get; set; }
        public string? DeliverNo { get; set; }
        public string? DepDesc { get; set; }
        public string? EmpName { get; set; }
    }
}
