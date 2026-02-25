using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Store
    {
        public int Id { get; set; }
        public int StoreCode { get; set; }
        public string StoreDesc { get; set; }
        public DateTime? SysDate { get; set; }
        public int InNum { get; set; }
        public int OutNum { get; set; }
        public int ToNum { get; set; }
        public int BackNum { get; set; }
        public string SysLock { get; set; }
        public int BackNum2 { get; set; }
        public int ScrapNum { get; set; }
    }
}
