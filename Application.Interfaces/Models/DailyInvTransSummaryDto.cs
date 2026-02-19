namespace Application.Interfaces.Models
{
    public class DailyInvTransSummaryDto
    {
        public decimal Inward { get; set; }
        public decimal TransferIn { get; set; }
        public decimal ReturnToStock { get; set; }
        public decimal Outward { get; set; }
        public decimal TransferOut { get; set; }
        public decimal ReturnToSupplier { get; set; }
        public decimal DeadStock { get; set; }
    }
}
