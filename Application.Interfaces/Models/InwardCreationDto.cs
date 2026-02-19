using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Models
{
    public class InwardCreationDto
    {
        public int SuplierCode { get; set; }
        public int DeptCode { get; set; }
        public int EmpCode { get; set; }
        public int TrType { get; set; }
        public string? BillNum { get; set; }
        public DateTime DeliverDate { get; set; }
        public List<InwardItemDto>? Items { get; set; }
    }
}
