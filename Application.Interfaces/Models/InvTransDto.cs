using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Interfaces.Models
{
    public class InvTransDto
    {
        public int StoreCode { get; set; }
        public int TrType { get; set; }
        public DateTime TrDate { get; set; }
        public int TrNum { get; set; }
        public int TrSerial { get; set; }
        public string ItemCode { get; set; }
        public int? DepCode { get; set; }
        public int? EmpCode { get; set; }
        public int? SuplierCode { get; set; }
        public int? FromToStore { get; set; }
        public decimal ItemQnt { get; set; }
        public decimal? ItemPrice { get; set; }
        public int? BillNum { get; set; }
        public int? TrNum2 { get; set; }
        public DateTime? TrDate2 { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? DeliverNo { get; set; }
        public DateTime? DeliverDate { get; set; }
    }
}
