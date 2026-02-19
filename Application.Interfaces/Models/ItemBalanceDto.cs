using System;

namespace Application.Interfaces.Models
{
    public class ItemBalanceDto
    {
        public int StoreCode { get; set; }
        public DateTime BalDate { get; set; }
        public string ItemCode { get; set; }
        public decimal OpenBal { get; set; }
        public decimal ItemIn { get; set; }
        public decimal ItemOut { get; set; }
        public decimal ItemFrom { get; set; }
        public decimal ItemTo { get; set; }
        public decimal ItemBack { get; set; }
        public decimal CurrentBal { get; set; }
        public decimal ItemBack2 { get; set; }
        public decimal ItemScrap { get; set; }
    }
}
