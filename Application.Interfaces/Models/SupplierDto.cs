using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Interfaces.Models
{
    public class SupplierDto
    {
        public int? SuplierCode { get; set; }
        public string? SuplierDesc { get; set; }
        public string? SuplierAddress { get; set; }
        public string? SuplierTel { get; set; }
        public string? SuplierFax { get; set; }
        public string? SuplierEmail { get; set; }
        public string? SuplierActivity { get; set; }
    }
}
