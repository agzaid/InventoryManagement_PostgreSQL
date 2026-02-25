using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string CatgryCode { get; set; }
        public string ItemDesc { get; set; }
        public decimal? RecallPrc { get; set; }
        public decimal? RecallQnt { get; set; }
        public string Barecode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
    }
}
