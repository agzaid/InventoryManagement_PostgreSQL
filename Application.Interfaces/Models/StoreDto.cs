using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Interfaces.Models
{
    public class StoreDto
    {
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
