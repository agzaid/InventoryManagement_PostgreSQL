using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public int DepCode { get; set; }
        public string DepDesc { get; set; }
        public string SectorCode { get; set; }
        public string GeneralManeg { get; set; }
        public string ManegSupCode { get; set; }
    }
}
