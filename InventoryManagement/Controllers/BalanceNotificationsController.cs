using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagement.Controllers
{
    [Authorize]
    public class BalanceNotificationsController : BaseController
    {
        private readonly ILowStockService _lowStockService;
        private readonly ILogger<BalanceNotificationsController> _logger;

        public BalanceNotificationsController(
            ILowStockService lowStockService,
            ILogger<BalanceNotificationsController> logger)
        {
            _lowStockService = lowStockService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? storeCode, int pageNumber = 1, int pageSize = 20, string searchTerm = null)
        {
            try
            {
                var pagedItems = await _lowStockService.GetItemsWithStockStatusAsync(pageNumber, pageSize, storeCode, searchTerm);
                ViewBag.StoreCode = storeCode;
                ViewBag.SearchTerm = searchTerm;
                return View(pagedItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading balance notifications");
                TempData["Error"] = "حدث خطأ أثناء تحميل البيانات";
                return View(new Application.Interfaces.Models.Pagination.PagedResult<LowStockNotificationDto> 
                { 
                    Items = new List<LowStockNotificationDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = pageSize
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateThreshold([FromBody] UpdateItemThresholdDto dto)
        {
            try
            {
                if (dto.MinimumQuantity < 0)
                {
                    return Json(new { success = false, message = "الحد الأدنى للكمية يجب أن يكون أكبر من أو يساوي صفر" });
                }

                if (dto.NotificationPercentage < 0 || dto.NotificationPercentage > 100)
                {
                    return Json(new { success = false, message = "نسبة التنبيه يجب أن تكون بين 0 و 100" });
                }

                var result = await _lowStockService.UpdateItemThresholdsAsync(dto);

                if (result)
                {
                    return Json(new { success = true, message = "تم تحديث الإعدادات بنجاح" });
                }
                else
                {
                    return Json(new { success = false, message = "فشل تحديث الإعدادات" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item threshold");
                return Json(new { success = false, message = "حدث خطأ أثناء التحديث" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateThresholdsBatch([FromBody] List<UpdateItemThresholdDto> dtos)
        {
            try
            {
                var invalidItems = dtos.Where(d => 
                    d.MinimumQuantity < 0 || 
                    d.NotificationPercentage < 0 || 
                    d.NotificationPercentage > 100).ToList();

                if (invalidItems.Any())
                {
                    return Json(new { success = false, message = "بعض القيم غير صحيحة" });
                }

                var result = await _lowStockService.UpdateItemThresholdsBatchAsync(dtos);

                if (result)
                {
                    return Json(new { success = true, message = $"تم تحديث {dtos.Count} صنف بنجاح" });
                }
                else
                {
                    return Json(new { success = false, message = "فشل تحديث الإعدادات" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item thresholds batch");
                return Json(new { success = false, message = "حدث خطأ أثناء التحديث" });
            }
        }

        public async Task<IActionResult> LowStockItems(int? storeCode)
        {
            try
            {
                var items = await _lowStockService.GetLowStockItemsAsync(storeCode);
                return View(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading low stock items");
                TempData["Error"] = "حدث خطأ أثناء تحميل البيانات";
                return View(new List<LowStockNotificationDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSummary(int? storeCode)
        {
            try
            {
                var summary = await _lowStockService.GetLowStockSummaryAsync(storeCode);
                return Json(new { success = true, data = summary });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock summary");
                return Json(new { success = false, message = "حدث خطأ أثناء تحميل الملخص" });
            }
        }
    }
}
