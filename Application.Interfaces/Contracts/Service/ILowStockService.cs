using Application.Interfaces.Models;
using Application.Interfaces.Models.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Contracts.Service
{
    public interface ILowStockService
    {
        Task<PagedResult<LowStockNotificationDto>> GetItemsWithStockStatusAsync(int pageNumber, int pageSize, int? storeCode = null, string searchTerm = null);
        Task<List<LowStockNotificationDto>> GetLowStockItemsAsync(int? storeCode = null);
        Task<LowStockSummaryDto> GetLowStockSummaryAsync(int? storeCode = null);
        Task<bool> UpdateItemThresholdsAsync(UpdateItemThresholdDto dto);
        Task<bool> UpdateItemThresholdsBatchAsync(List<UpdateItemThresholdDto> dtos);
        Task<LowStockNotificationDto> GetItemStockStatusAsync(int itemId, int? storeCode = null);
    }
}
