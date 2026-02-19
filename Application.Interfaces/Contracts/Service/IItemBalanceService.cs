using Application.Interfaces.Models;

namespace Application.Interfaces.Contracts.Service
{
    public interface IItemBalanceService
    {
        Task<ItemBalanceDto?> GetItemBalanceByIdAsync(int storeCode, string itemCode);
        Task<IReadOnlyList<ItemBalanceDto>> GetAllItemBalancesAsync();
        Task<IReadOnlyList<ItemBalanceDto>> GetItemBalancesByStoreAsync(int storeCode);
        Task<int> CreateItemBalanceAsync(ItemBalanceDto command);
        Task UpdateItemBalanceAsync(ItemBalanceDto command);
        Task DeleteItemBalanceAsync(int storeCode, string itemCode);
        Task<CurrentStockDto> GetCurrentStockSummaryAsync(int storeCode, string? itemCode = null);
        Task<IReadOnlyList<LowStockItemDto>> GetLowStockItemsAsync(int storeCode, decimal threshold = 20, int? limit = null);
        Task<int> GetLowStockCountAsync(int storeCode, decimal threshold = 20);
        Task<decimal> GetTotalCurrentBalanceAsync(int storeCode);
    }
}
