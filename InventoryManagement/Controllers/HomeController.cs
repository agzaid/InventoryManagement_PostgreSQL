using Application.Interfaces.Contracts.Localization;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using InventoryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using System.Globalization;

namespace InventoryManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAppLocalizer _localizer;
        private readonly IItemBalanceService _itemBalanceService;
        private readonly IInvTransService _invTransService;
        private readonly IItemService _itemService;
        private readonly IItemCategoryService _itemCategoryService;
        private readonly ISupplierService _supplierService;

        public HomeController(ILogger<HomeController> logger, IAppLocalizer localizer, IItemBalanceService itemBalanceService, IInvTransService invTransService, IItemService itemService, IItemCategoryService itemCategoryService, ISupplierService supplierService)
        {
            _logger = logger;
            _localizer = localizer;
            _itemBalanceService = itemBalanceService;
            _invTransService = invTransService;
            _itemService = itemService;
            _itemCategoryService = itemCategoryService;
            _supplierService = supplierService;
        }
        public IActionResult Index(string errorMessage = null)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ViewBag.ErrorMessage = errorMessage;
            }
            var culture = CultureInfo.CurrentCulture.Name;

            ViewData["Title"] = _localizer["Save"];
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ConfigureSystemSettings()
        {
            return View();
        }
        public IActionResult PostDailyTransactions()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet("throw")]
        public IActionResult ThrowError()
        {
            // This exception will be uncaught by the framework and passed to your middleware
            throw new InvalidOperationException("This is a deliberate test exception to verify logging and handling.");
        }

        [HttpGet]
        public async Task<IActionResult> GetLowStockItems(decimal threshold = 20, int? limit = 5)
        {
            var lowStockItems = await _itemBalanceService.GetLowStockItemsAsync(1, threshold, limit);
            return Json(new { success = true, data = lowStockItems });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLowStockItems(decimal threshold = 20)
        {
            var lowStockItems = await _itemBalanceService.GetLowStockItemsAsync(1, threshold, null);
            return Json(new { success = true, data = lowStockItems });
        }

        [HttpGet]
        public async Task<IActionResult> GetLowStockCount(decimal threshold = 20)
        {
            var count = await _itemBalanceService.GetLowStockCountAsync(1, threshold);
            return Json(new { success = true, count = count });
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentActivityToday(int limit = 10)
        {
            var data = await _invTransService.GetRecentActivityTodayAsync(1, limit);
            return Json(new { success = true, data });
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalAssets = await _itemBalanceService.GetTotalCurrentBalanceAsync(1);
            var totalSubCategory = await _itemCategoryService.GetTotalCountAsync();
            var totalSupplier = await _supplierService.GetTotalCountAsync();

            return Json(new
            {
                success = true,
                data = new
                {
                    totalAssets,
                    totalSubCategory,
                    totalSupplier
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoryDistribution()
        {
            var data = await _itemCategoryService.GetCategoryDistributionAsync();
            return Json(new { success = true, data });
        }
    }
}
