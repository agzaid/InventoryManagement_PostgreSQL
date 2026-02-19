using Application.Interfaces.Models;

namespace Application.Interfaces.Contracts.Service
{
    public interface IMonthlyBalanceService
    {
        Task<MonthlyBalanceDto?> GetMonthlyBalanceByIdAsync(int storeCode, int year, int month, string itemCode);
        Task<MonthlyConsumptionSummaryDto> GetMonthlyConsumptionSummaryAsync(int storeCode, int year, int month, int top = 10);
        Task<IReadOnlyList<MonthlyBalanceDto>> GetAllMonthlyBalancesAsync();
        Task<IReadOnlyList<MonthlyBalanceDto>> GetMonthlyBalancesByStoreAsync(int storeCode);
        Task<IReadOnlyList<MonthlyBalanceDto>> GetMonthlyBalancesByYearMonthAsync(int storeCode, int year, int month);
        Task<int> CreateMonthlyBalanceAsync(MonthlyBalanceDto command);
        Task UpdateMonthlyBalanceAsync(MonthlyBalanceDto command);
        Task DeleteMonthlyBalanceAsync(int storeCode, int year, int month, string itemCode);
    }
}
