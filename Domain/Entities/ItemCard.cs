using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ItemCard
    {
        public int Id { get; set; }
        public int? StoreCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public DateTime? CardDate { get; set; }
        public string CardMemo { get; set; }
        public int? InNum { get; set; }
        public decimal? InQnt { get; set; }
        public decimal? InPrice { get; set; }
        public int? OutNum { get; set; }
        public decimal? OutQnt { get; set; }
        public decimal? CardBalance { get; set; }
        public int? CardSerial { get; set; }
    }
}
