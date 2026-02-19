namespace Application.Interfaces.Models
{
    public class CategoryDistributionDto
    {
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public int ItemCount { get; set; }
        public decimal Percentage { get; set; }
    }
}
