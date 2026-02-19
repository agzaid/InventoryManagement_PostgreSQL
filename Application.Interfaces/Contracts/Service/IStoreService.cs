using Application.Interfaces.Models;

namespace Application.Interfaces.Contracts.Service
{
    public interface IStoreService
    {
        Task<StoreDto> GetSystemSettingsAsync();
        Task<int> UpdateSystemSettingsAsync(StoreDto settings);
    }
}
