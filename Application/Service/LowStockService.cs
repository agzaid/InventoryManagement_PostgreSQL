using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using Application.Interfaces.Models.Pagination;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Service
{
    public class LowStockService : ILowStockService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IItemBalanceRepository _itemBalanceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LowStockService(
            IItemRepository itemRepository,
            IItemBalanceRepository itemBalanceRepository,
            IUnitOfWork unitOfWork)
        {
            _itemRepository = itemRepository;
            _itemBalanceRepository = itemBalanceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<LowStockNotificationDto>> GetItemsWithStockStatusAsync(int pageNumber, int pageSize, int? storeCode = null, string searchTerm = null)
        {
            var itemsQuery = _itemRepository.GetQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                itemsQuery = itemsQuery.Where(i =>
                    i.ItemCode.ToLower().Contains(searchTerm) ||
                    i.ItemDesc.ToLower().Contains(searchTerm));
            }

            var totalItems = await itemsQuery.CountAsync();

            var items = await itemsQuery
                .Include(i => i.ItemCategory)
                .OrderBy(i => i.ItemCode)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var balances = await _itemBalanceRepository.GetAllAsync();

            if (storeCode.HasValue)
            {
                balances = balances.Where(b => b.StoreCode == storeCode.Value).ToList();
            }

            var latestBalances = balances
                .GroupBy(b => b.ItemCode)
                .Select(g => g.OrderByDescending(b => b.BalDate).FirstOrDefault())
                .Where(b => b != null)
                .ToList();

            var result = new List<LowStockNotificationDto>();

            foreach (var item in items)
            {
                var balance = latestBalances.FirstOrDefault(b => b.ItemCode == item.ItemCode);
                var currentQty = balance?.CurrentBal ?? 0;
                var openingQty = balance?.OpenBal ?? 0;

                var dto = new LowStockNotificationDto
                {
                    Id = item.Id,
                    ItemCode = item.ItemCode,
                    ItemDesc = item.ItemDesc,
                    CategoryName = item.ItemCategory?.CatgryDesc ?? "",
                    CurrentQuantity = currentQty,
                    MinimumQuantity = item.MinimumQuantity,
                    NotificationPercentage = item.NotificationPercentage,
                    IsLowStock = IsLowStock(currentQty, item.MinimumQuantity, item.NotificationPercentage, openingQty),
                    StockStatus = GetStockStatus(currentQty, item.MinimumQuantity, item.NotificationPercentage, openingQty),
                    StockPercentage = CalculateStockPercentage(currentQty, item.MinimumQuantity)
                };

                result.Add(dto);
            }

            var orderedResult = result.OrderBy(r => r.IsLowStock ? 0 : 1)
                        .ThenBy(r => r.StockPercentage)
                        .ToList();

            return new PagedResult<LowStockNotificationDto>
            {
                Items = orderedResult,
                TotalCount = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<List<LowStockNotificationDto>> GetLowStockItemsAsync(int? storeCode = null)
        {
            var allItemsResult = await GetItemsWithStockStatusAsync(1, int.MaxValue, storeCode);
            return allItemsResult.Items.Where(i => i.IsLowStock).ToList();
        }

        public async Task<LowStockSummaryDto> GetLowStockSummaryAsync(int? storeCode = null)
        {
            var allItemsResult = await GetItemsWithStockStatusAsync(1, int.MaxValue, storeCode);
            var allItems = allItemsResult.Items;
            var lowStockItems = allItems.Where(i => i.IsLowStock).ToList();
            var criticalItems = allItems.Where(i => i.CurrentQuantity <= i.MinimumQuantity).ToList();

            return new LowStockSummaryDto
            {
                TotalItems = allItems.Count,
                LowStockItems = lowStockItems.Count,
                CriticalStockItems = criticalItems.Count,
                LowStockPercentage = allItems.Count > 0
                    ? Math.Round((decimal)lowStockItems.Count / allItems.Count * 100, 2)
                    : 0
            };
        }

        public async Task<bool> UpdateItemThresholdsAsync(UpdateItemThresholdDto dto)
        {
            if (dto.MinimumQuantity < 0 || dto.NotificationPercentage < 0 || dto.NotificationPercentage > 100)
            {
                return false;
            }

            var item = await _itemRepository.GetAsyncById(dto.ItemId);
            if (item == null)
            {
                return false;
            }

            item.MinimumQuantity = dto.MinimumQuantity;
            item.NotificationPercentage = dto.NotificationPercentage;

            await _itemRepository.UpdateAsync(item);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            else
                return false;
        }

        public async Task<bool> UpdateItemThresholdsBatchAsync(List<UpdateItemThresholdDto> dtos)
        {
            foreach (var dto in dtos)
            {
                if (dto.MinimumQuantity < 0 || dto.NotificationPercentage < 0 || dto.NotificationPercentage > 100)
                {
                    continue;
                }

                var item = await _itemRepository.GetAsyncById(dto.ItemId);
                if (item != null)
                {
                    item.MinimumQuantity = dto.MinimumQuantity;
                    item.NotificationPercentage = dto.NotificationPercentage;
                    await _itemRepository.UpdateAsync(item);
                    var result = await _unitOfWork.SaveChangesAsync();
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }

            return true;
        }

        public async Task<LowStockNotificationDto> GetItemStockStatusAsync(int itemId, int? storeCode = null)
        {
            var allItemsResult = await GetItemsWithStockStatusAsync(1, int.MaxValue, storeCode);
            return allItemsResult.Items.FirstOrDefault(i => i.Id == itemId);
        }

        private bool IsLowStock(decimal currentQty, decimal minimumQty, decimal notificationPercentage, decimal openingQty)
        {
            if (minimumQty > 0)
            {
                return currentQty <= minimumQty;
            }

            if (notificationPercentage > 0 && openingQty > 0)
            {
                var threshold = openingQty * (notificationPercentage / 100);
                return currentQty <= threshold;
            }

            return false;
        }

        private string GetStockStatus(decimal currentQty, decimal minimumQty, decimal notificationPercentage, decimal openingQty)
        {
            if (minimumQty > 0)
            {
                if (currentQty <= minimumQty)
                {
                    return "Critical";
                }
                return "Normal";
            }

            if (notificationPercentage > 0 && openingQty > 0)
            {
                var threshold = openingQty * (notificationPercentage / 100);
                if (currentQty <= threshold)
                {
                    return "Low";
                }
            }

            return "Normal";
        }

        private decimal CalculateStockPercentage(decimal currentQty, decimal minimumQty)
        {
            if (minimumQty == 0)
            {
                return 100;
            }

            var percentage = (currentQty / minimumQty) * 100;
            return Math.Round(percentage, 2);
        }
    }
}
