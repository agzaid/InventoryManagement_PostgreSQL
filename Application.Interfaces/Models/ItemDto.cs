using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Models
{
    public class ItemDto
    {
        public string? ItemCode { get; set; }
        public string? CatgryCode { get; set; }
        public string? ItemDesc { get; set; }
        public decimal? RecallPrc { get; set; }
        public decimal? RecallQnt { get; set; }
        public string? Barecode { get; set; }
        public ItemCategoryDto? ItemCategory { get; set; }

    }
}
