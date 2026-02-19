using Application.Interfaces.Models;

namespace Application.Interfaces.Contracts.Service
{
    public interface IMonthlyConsumService
    {
        Task<MonthlyConsumDto?> GetMonthlyConsumByIdAsync(int storeCode, int year, int month, int depCode, string itemCode);
        Task<IReadOnlyList<MonthlyConsumDto>> GetAllMonthlyConsumsAsync();
        Task<IReadOnlyList<MonthlyConsumDto>> GetMonthlyConsumsByStoreAsync(int storeCode);
        Task<IReadOnlyList<MonthlyConsumDto>> GetMonthlyConsumsByYearMonthAsync(int storeCode, int year, int month);
        Task<int> CreateMonthlyConsumAsync(MonthlyConsumDto command);
        Task UpdateMonthlyConsumAsync(MonthlyConsumDto command);
        Task DeleteMonthlyConsumAsync(int storeCode, int year, int month, int depCode, string itemCode);
    }
}
