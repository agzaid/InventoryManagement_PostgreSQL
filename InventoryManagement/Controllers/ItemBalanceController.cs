using Application.Interfaces.Contracts.Localization;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using Domain.Exceptions;
using InventoryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace InventoryManagement.Controllers
{
    public class ItemBalanceController : Controller
    {
        private readonly ILogger<ItemBalanceController> _logger;
        private readonly IAppLocalizer _localizer;
        private readonly IInvUserService _userService;
        private readonly ISystemManagementService _sysManagement;
        private readonly IItemBalanceService _itemBalanceService;
        private readonly IItemService _itemService;

        public ItemBalanceController(ILogger<ItemBalanceController> logger, IAppLocalizer localizer,IInvUserService userService, ISystemManagementService sysManagement, IItemBalanceService itemBalanceService, IItemService itemService)
        {
            _logger = logger;
            _localizer = localizer;
            _userService = userService;
            _sysManagement = sysManagement;
            _itemBalanceService = itemBalanceService;
            _itemService = itemService;
        }
        public async Task<IActionResult> CurrentBalance()
        {
            var stockSummary = await _itemBalanceService.GetCurrentStockSummaryAsync(1); // storeCode = 1
            var items = await _itemService.GetAllItemAsync();
            ViewBag.Items = items;
            return View(stockSummary);
        }

        [HttpGet]
        public async Task<IActionResult> GetItemBalance(string itemCode)
        {
            if (string.IsNullOrEmpty(itemCode))
            {
                return Json(new { success = false, message = "يجب اختيار صنف" });
            }

            var stockSummary = await _itemBalanceService.GetCurrentStockSummaryAsync(1, itemCode); // storeCode = 1
            return Json(new { success = true, data = stockSummary });
        }

        [HttpGet]
        public async Task<IActionResult> GetInventoryData(string viewType = "specific", string? searchTerm = null, int page = 1, int pageSize = 20, int? warehouseId = null)
        {
            int storeCode = warehouseId ?? 1;

            var balances = await _itemBalanceService.GetItemBalancesByStoreAsync(storeCode);
            var items = await _itemService.GetAllItemAsync();
            var itemDict = items.ToDictionary(i => i.ItemCode ?? string.Empty, i => i);

            // Take the most recent balance per item (by BalDate)
            var latestPerItem = balances
                .OrderByDescending(b => b.BalDate)
                .GroupBy(b => b.ItemCode)
                .Select(g => g.First())
                .ToList();

            IEnumerable<ItemBalanceDto> filtered = latestPerItem;

            if (!string.IsNullOrWhiteSpace(viewType) && viewType.ToLower() == "specific" && !string.IsNullOrWhiteSpace(searchTerm))
            {
                var q = searchTerm.Trim().ToLower();
                filtered = latestPerItem.Where(b =>
                    (!string.IsNullOrEmpty(b.ItemCode) && b.ItemCode.ToLower().Contains(q))
                    || (itemDict.TryGetValue(b.ItemCode ?? string.Empty, out var itm) && !string.IsNullOrEmpty(itm.ItemDesc) && itm.ItemDesc.ToLower().Contains(q))
                );
            }

            var totalCount = filtered.Count();

            var paged = filtered
                .OrderBy(b => b.ItemCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new {
                    ItemCode = b.ItemCode,
                    ItemName = itemDict.TryGetValue(b.ItemCode ?? string.Empty, out var it) ? (it.ItemDesc ?? b.ItemCode) : b.ItemCode,
                    CurrentBalance = b.CurrentBal,
                    LastAuditDate = b.BalDate.ToString("yyyy-MM-dd")
                })
                .ToList();

            return Json(new { success = true, items = paged, totalCount });
        }

        public async Task<IActionResult> Inventory(string viewType = "specific", string? searchTerm = null, int? warehouseId = null)
        {
            int storeCode = warehouseId ?? 1;

            // Get balances and items
            var balances = await _itemBalanceService.GetItemBalancesByStoreAsync(storeCode);
            var items = await _itemService.GetAllItemAsync();
            var itemDict = items.ToDictionary(i => i.ItemCode ?? string.Empty, i => i);

            // return recent per item for server-render fallback
            var latestPerItem = balances
                .OrderByDescending(b => b.BalDate)
                .GroupBy(b => b.ItemCode)
                .Select(g => g.First())
                .ToList();

            IEnumerable<ItemBalanceDto> filtered = latestPerItem;

            if (!string.IsNullOrWhiteSpace(viewType) && viewType.ToLower() == "specific" && !string.IsNullOrWhiteSpace(searchTerm))
            {
                var q = searchTerm.Trim().ToLower();

                // filter by code contains or name contains
                filtered = latestPerItem.Where(b =>
                    (!string.IsNullOrEmpty(b.ItemCode) && b.ItemCode.ToLower().Contains(q))
                    || (itemDict.TryGetValue(b.ItemCode ?? string.Empty, out var itm) && !string.IsNullOrEmpty(itm.ItemDesc) && itm.ItemDesc.ToLower().Contains(q))
                ).ToList();
            }

            var vm = filtered.Select(b => new InventoryViewModel
            {
                ItemCode = b.ItemCode,
                ItemName = itemDict.TryGetValue(b.ItemCode ?? string.Empty, out var it) ? (it.ItemDesc ?? b.ItemCode) : b.ItemCode,
                CurrentBalance = b.CurrentBal,
                LastAuditDate = b.BalDate
            }).ToList();

            // Preserve query values for the view
            ViewBag.SelectedViewType = viewType;
            ViewBag.SearchTerm = searchTerm ?? string.Empty;
            ViewBag.WarehouseId = warehouseId;

            return View(vm);
        }
       
        [HttpPost]
        public async Task<IActionResult> SaveUser([FromBody] InvUserDto usersData)
        {
                if (usersData == null)
                throw new BadRequestException("لم يتم استقبال بيانات");
            if (!ModelState.IsValid)
            {
                // Extract the first error message and throw it
                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                throw new BadRequestException(firstError ?? "بيانات غير صالحة");
            }

            var result = await _sysManagement.CreateInvUserAsync(usersData);

            return Json(new { success = true, message = "تم إضافة البيانات بنجاح" });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] InvUserDto usersData)
        {
            if (usersData == null)
            {
                throw new BadRequestException("NoDataReceived");
            }
            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                throw new BadRequestException(firstError ?? "InvalidData");
            }
            await _sysManagement.UpdateInvUserAsync(usersData);
            return Json(new
            {
                success = true,
                message = _localizer["UpdateSuccess"]
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] InvUserDto data)
        {
            // 1. Basic check
            if (data.UserCode <= 0)
            {
                throw new BadRequestException("InvalidUserCode"); // Key for localization
            }
            await _sysManagement.DeleteInvUserAsync((int)data.UserCode);

            // 3. Success Response
            return Json(new
            {
                success = true,
                message = _localizer["DeleteSuccess"]
            });
        }
    }
}
