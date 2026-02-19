using Application.Interfaces.Models;

namespace Application.Interfaces.Contracts.Service
{
    public interface IItemCategoryService
    {
        Task<IReadOnlyList<ItemCategoryDto>> GetAllItemCategoryAsync();
        Task<ItemCategoryDto?> GetItemCategoryByIdAsync(int id);
        Task<string> CreateItemCategoryAsync(ItemCategoryDto command);
        Task UpdateItemCategoryAsync(ItemCategoryDto command);
        Task DeleteItemCategoryAsync(string id);
        Task<int> GetTotalCountAsync();
        Task<IReadOnlyList<CategoryDistributionDto>> GetCategoryDistributionAsync();
    }
}
