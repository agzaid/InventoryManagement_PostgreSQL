using Application.Interfaces.Contracts.Localization;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InventoryManagement.Controllers
{
    public class SystemManagementController : Controller
    {
        private readonly ILogger<SystemManagementController> _logger;
        private readonly IAppLocalizer _localizer;
        private readonly IStoreService _store;
        private readonly IInvUserService _userService;
        private readonly ISystemManagementService _sysManagement;
        private readonly IInvTransService _invTransService;
        private readonly IMonthlyBalanceService _monthlyBalanceService;

        public SystemManagementController(ILogger<SystemManagementController> logger, IAppLocalizer localizer, IStoreService store, IInvUserService userService, ISystemManagementService sysManagement, IInvTransService invTransService, IMonthlyBalanceService monthlyBalanceService)
        {
            _logger = logger;
            _localizer = localizer;
            _store = store;
            _userService = userService;
            _sysManagement = sysManagement;
            _invTransService = invTransService;
            _monthlyBalanceService = monthlyBalanceService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _sysManagement.GetAllEgxEmployeesAsync());
        }
        public async Task<IActionResult> ConfigureSystemSettings()
        {
            return View(await _store.GetSystemSettingsAsync());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSettings(StoreDto model)
        {
            var success = await _store.UpdateSystemSettingsAsync(model);

            if (success > 0)
            {
                return Json(new { success = true, message = "تم تحديث إعدادات النظام بنجاح" });
            }

            return Json(new { success = false, message = "فشل تحديث البيانات، يرجى المحاولة لاحقاً." });

        }

        public IActionResult PostDailyTransactions()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDailyInvTransSummaryToday()
        {
            var settings = await _store.GetSystemSettingsAsync();
            int storeCode = settings.StoreCode;

            var data = await _invTransService.GetDailyInvTransSummaryTodayAsync(storeCode);
            return Json(new { success = true, data });
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyConsumptionSummary(int year, int month, int top = 10)
        {
            var settings = await _store.GetSystemSettingsAsync();
            int storeCode = settings.StoreCode;

            var data = await _monthlyBalanceService.GetMonthlyConsumptionSummaryAsync(storeCode, year, month, top);
            return Json(new { success = true, data });
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
            if (data.UserCode <= 0)
            {
                throw new BadRequestException("InvalidUserCode");
            }
            await _sysManagement.DeleteInvUserAsync((int)data.UserCode);

            // 3. Success Response
            return Json(new
            {
                success = true,
                message = _localizer["DeleteSuccess"]
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunRelayProcess()
        {
            // الحصول على كود المستودع الحالي (من الجلسة أو إعدادات المستخدم)
            var settings = await _store.GetSystemSettingsAsync();
            int storeCode = settings.StoreCode;
            DateTime systemDate = settings.SysDate ?? DateTime.Now;

            var result = await _sysManagement.RelayDataAsync(storeCode, systemDate);

            if (result.Success)
            {
                return Json(new { success = true, message = result.Message });
            }

            return Json(new { success = false, message = result.Message });
        }
    }
}
