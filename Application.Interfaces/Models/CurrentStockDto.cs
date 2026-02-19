namespace Application.Interfaces.Models
{
    public class CurrentStockDto
    {
        // Today's Movement (from GetItemBalance/transactions)
        public decimal TodayInward { get; set; }
        public decimal TodayOutward { get; set; }
        public decimal TodayTransFrom { get; set; }
        public decimal TodayTransTo { get; set; }
        public decimal TodayReturnIn { get; set; }
        public decimal TodayReturnOut { get; set; }
        public decimal TodayStagnant { get; set; }
        public decimal TodayBalance { get; set; }
        public decimal SecondInward { get; set; }
        public decimal SecondOutward { get; set; }
        public decimal SecondTransFrom { get; set; }
        public decimal SecondTransTo { get; set; }
        public decimal SecondReturnIn { get; set; }
        public decimal SecondReturnOut { get; set; }
        public decimal SecondStagnant { get; set; }
        public decimal SecondBalance { get; set; }

        // Previous Balance (from ItemBalance table)
        public decimal OpeningBalance { get; set; }
        public decimal PrevInward { get; set; }
        public decimal PrevOutward { get; set; }
        public decimal PrevTransFrom { get; set; }
        public decimal PrevTransTo { get; set; }
        public decimal PrevReturnIn { get; set; }
        public decimal PrevReturnOut { get; set; }
        public decimal PrevStagnant { get; set; }
        public decimal TotalStockBalance { get; set; }
    }
}
