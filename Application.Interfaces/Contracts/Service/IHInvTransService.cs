using Application.Interfaces.Models;
using Application.Interfaces.Models.Pagination;
using Domain.Enums;

namespace Application.Interfaces.Contracts.Service
{
    public interface IHInvTransService
    {
        Task<int> CreateHInvTransAsync(InwardCreationDto command);
        Task<HInvTransDto?> GetHInvTransByIdAsync(int id);
        Task<IReadOnlyList<HInvTransDto>> GetAllHInvTransAsync();
        Task UpdateHInvTransAsync(HInvTransDto command);
        Task DeleteHInvTransAsync(int id);
        Task<List<TransactionDisplayDto>> GetHistoryTransactionsEmployeeAsync(int storeCode);
        Task<List<TransactionDisplayDto>> GetHistoryByTypeAsync(int storeCode, TrType type);
        Task<List<TransactionDisplayDto>> GetHistoryTransactionsByTypeAndDateAsync(int storeCode, int trType, DateTime from, DateTime to);
        Task<PagedResult<TransactionDisplayDto>> GetHistoryTransactionsPaginatedAsync(List<int> trTypes, int page, int pageSize, DateTime start, DateTime end);
    }
}
