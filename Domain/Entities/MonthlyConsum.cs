using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class MonthlyConsum
    {
        public int StoreCode { get; set; }
        public int ConsumYear { get; set; }
        public int ConsumMonth { get; set; }
        public int DepCode { get; set; }
        public string ItemCode { get; set; }
        public decimal ConsumQnt { get; set; }
        public decimal ConsumAvg { get; set; }
    }
}
