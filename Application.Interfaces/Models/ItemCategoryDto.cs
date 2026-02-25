using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Models
{
    public class ItemCategoryDto
    {
        public int? Id { get; set; }
        public int? CatgryCode { get; set; }
        public string? CatgryDesc { get; set; }
    }
}
