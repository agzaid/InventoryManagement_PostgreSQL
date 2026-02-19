using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{

    public class ItemCategory
    {
        public string CatgryCode { get; set; }
        public string CatgryDesc { get; set; }
        public virtual List<Item> Items { get; set; }
    }
}
