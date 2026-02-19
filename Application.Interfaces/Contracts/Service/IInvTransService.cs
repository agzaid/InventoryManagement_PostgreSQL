using Application.Interfaces.Models;

namespace Application.Interfaces.Contracts.Service
{
    public interface IInvTransService
    {
        Task<int> CreateInvTransAsync(InwardCreationDto command);
        Task<InvTransDto?> GetInvTransByIdAsync(int id);
        Task<IReadOnlyList<InvTransDto>> GetAllInvTransAsync();
        Task UpdateInvTransAsync(InvTransDto command);
        Task DeleteInvTransAsync(int id);
        Task<List<TransactionDisplayDto>> GetInventoryTransactionsAsync(int storeCode);
        
        //others are not correct
        //Task<int> CreateInvTransAsync(InvTransDto command);
        Task<List<TransactionDisplayDto>> GetInventoryTransactionsEmployeeAsync(int storeCode);
        
        // Get transactions by TrType
        Task<List<TransactionDisplayDto>> GetTransactionsByTypeAsync(int storeCode, int trType);
        Task<IReadOnlyList<RecentActivityDto>> GetRecentActivityTodayAsync(int storeCode, int limit = 10);
        Task<DailyInvTransSummaryDto> GetDailyInvTransSummaryTodayAsync(int storeCode);
    }
}
