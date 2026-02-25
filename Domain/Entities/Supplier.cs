using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Supplier
    {
        public int Id { get; set; }
        public int SuplierCode { get; set; }
        public string SuplierDesc { get; set; }
        public string SuplierAddress { get; set; }
        public string SuplierTel { get; set; }
        public string SuplierFax { get; set; }
        public string SuplierEmail { get; set; }
        public string SuplierActivity { get; set; }
    }
}
