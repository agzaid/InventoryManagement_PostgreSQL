using Application.Interfaces.Models;
using Application.Interfaces.Models.Pagination;

namespace Application.Interfaces.Contracts.Service
{
    public interface IItemService
    {
        Task<IReadOnlyList<ItemDto>> GetAllItemAsync();
        Task<ItemDto?> GetItemByIdAsync(int id);
        Task<string> CreateItemAsync(ItemDto command);
        Task UpdateItemAsync(ItemDto command, string oldCategory);
        Task DeleteItemAsync(string id);
        Task<PagedResult<ItemDto>> GetItemsPaginated(int page, string search, string category);
        Task<int> GetTotalCountAsync();
    }
}
