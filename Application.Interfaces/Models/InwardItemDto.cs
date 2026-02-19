using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Models
{
    public class InwardItemDto
    {
        public string? ItemCode { get; set; }
        public decimal ItemQnt { get; set; }
        public decimal ItemPrice { get; set; }
    }
}
