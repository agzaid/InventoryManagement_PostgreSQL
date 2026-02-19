using System.Collections.Generic;

namespace Application.Interfaces.Models
{
    public class MonthlyConsumptionSummaryDto
    {
        public int StoreCode { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalConsumptionQnt { get; set; }
        public int ItemsCount { get; set; }
        public IReadOnlyList<MonthlyConsumptionItemDto> TopItems { get; set; }
    }
}
