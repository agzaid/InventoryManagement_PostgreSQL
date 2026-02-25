using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class OpenBalance
    {
        public int Id { get; set; }
        public int StoreCode { get; set; }
        public DateTime OpenDate { get; set; }
        public string ItemCode { get; set; }
        public decimal OpenBal { get; set; }
        public string RelayFlag { get; set; }
    }

}
