using Application.Interfaces.Contracts.Localization;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagement.Controllers
{
    public class InventoryMovementController : Controller
    {
        private readonly ILogger<InventoryMovementController> _logger;
        private readonly IAppLocalizer _localizer;
        private readonly IInvUserService _userService;
        private readonly ISupplierService _supplierService;
        private readonly IItemService _itemService;
        private readonly IInvTransService _invTransService;
        private readonly ISystemManagementService _sysManagement;
        private readonly IDepartmentService _departmentService;
        private readonly IEgxEmployeeService _egxEmployeeService;
        private readonly IHInvTransService _hInvTransService;

        public InventoryMovementController(ILogger<InventoryMovementController> logger, IAppLocalizer localizer, IInvUserService userService,
            ISupplierService supplierService, ISystemManagementService sysManagement, IItemService itemService,
            IInvTransService invTransService, IDepartmentService departmentService, IEgxEmployeeService egxEmployeeService, IHInvTransService hInvTransService)
        {
            _logger = logger;
            _localizer = localizer;
            _userService = userService;
            _supplierService = supplierService;
            _sysManagement = sysManagement;
            _itemService = itemService;
            _invTransService = invTransService;
            _departmentService = departmentService;
            _egxEmployeeService = egxEmployeeService;
            _hInvTransService = hInvTransService;
        }

        #region receive goods
        public async Task<IActionResult> Recordgoodsreceivedintothewarehouse()
        {
            var supplier = await _supplierService.GetAllSupplierAsync();
            var item = await _itemService.GetAllItemAsync();
            var invtrans = await _invTransService.GetTransactionsByTypeAsync(1, 1); // storeCode=1, trType=1 (Inward)
            ViewBag.supplier = supplier;
            ViewBag.items = item;
            ViewBag.TodayTransactions = invtrans;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(InwardCreationDto model)
        {
            if (model.Items == null || !model.Items.Any())
            {
                return RedirectToAction(nameof(Recordgoodsreceivedintothewarehouse));
            }
            var generatedTrNum = await _invTransService.CreateInvTransAsync(model);
            TempData["Model"] = generatedTrNum;
            return RedirectToAction("PrintInwardReceipt", new { trNum = generatedTrNum });
        }

        public async Task<IActionResult> PrintInwardReceipt(long trNum)
        {
            // جلب البيانات من السيرفس (تأكد من كتابة ميثود تجلب بيانات الفاتورة بهذا الرقم)
            var transactions = await _invTransService.GetInvTransByIdAsync((int)trNum);

            if (transactions == null)
                return NotFound("لم يتم العثور على الفاتورة المطلوبة");

            // إذا كانت صفحة الطباعة تتوقع كائن واحد (أول صنف)
            var model = new InvTransDto
            {
                // بيانات الحركة الأساسية
                StoreCode = 1,                          // مخزن رقم 1
                TrType = 1,                             // نوع الحركة: وارد
                TrNum = 10542,                          // رقم إذن الاستلام
                TrSerial = 1,                           // المسلسل الأول في الفاتورة
                TrDate = DateTime.Now,                  // تاريخ تسجيل النظام

                // بيانات المورد والأصناف
                SuplierCode = 4002,                     // كود المورد (مثلاً: شركة التوريدات الحديثة)
                ItemCode = "MT-9908",                   // كود الصنف (مثلاً: محرك كهربائي 5 حصان)
                ItemQnt = 25.0m,                        // الكمية المستلمة
                ItemPrice = 4500.50m,                   // سعر الوحدة

                // بيانات المستندات المرتبطة
                BillNum = 7741,                         // رقم فاتورة المورد الورقية
                OrderDate = DateTime.Now.AddDays(-3),   // تاريخ أمر الشراء الأصلي
                DeliverNo = 90012,                      // رقم إذن التسليم (Delivery Note)
                DeliverDate = DateTime.Now.AddDays(-1), // تاريخ وصول البضاعة فعلياً للمخزن

                // حقول اختيارية (تترك null في حالة الوارد من مورد)
                DepCode = null,                         // لا يوجد قسم لأنها عملية شراء
                EmpCode = 112,                          // كود الموظف المستلم (أمين المخزن)
                FromToStore = null                      // لا يوجد تحويل بين مخازن
            };
            return View("PrintReceiptTemplate", model);
        }

        #endregion
        #region issued goods
        public async Task<IActionResult> Recordgoodsissuedfromthewarehouse()
        {
            // var supplier = await _supplierService.GetAllSupplierAsync();
            var item = await _itemService.GetAllItemAsync();
            var invtrans = await _invTransService.GetTransactionsByTypeAsync(1, 2); // storeCode=1, trType=2 (Outgoing)
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var egxEmployees = await _egxEmployeeService.GetAllEgxEmployeeAsync();
            //  ViewBag.supplier = supplier;
            ViewBag.items = item;
            ViewBag.TodayTransactions = invtrans;
            ViewBag.departments = departments;
            ViewBag.egxEmployees = egxEmployees;
            return View(new List<CategoryDto>());
        }

        [HttpPost]
        public async Task<IActionResult> CreateWithReceiver(InwardCreationDto model)
        {
            if (model.Items == null || !model.Items.Any())
            {
                return RedirectToAction(nameof(Recordgoodsissuedfromthewarehouse));
            }
            var generatedTrNum = await _invTransService.CreateInvTransAsync(model);
            return RedirectToAction("Recordgoodsissuedfromthewarehouse");
        }
        #endregion
        #region transfer
        public async Task<IActionResult> Recordtransfersbetweenwarehouses()
        {
            var invtrans = await _invTransService.GetTransactionsByTypeAsync(1, 3); // storeCode=1, trType=3 (Transfer)
            var item = await _itemService.GetAllItemAsync();
            ViewBag.items = item;
            ViewBag.todayTransactions = invtrans;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransfer(InwardCreationDto model)
        {
            if (model.Items == null || !model.Items.Any())
            {
                return RedirectToAction(nameof(Recordtransfersbetweenwarehouses));
            }
            var generatedTrNum = await _invTransService.CreateInvTransAsync(model);
            return RedirectToAction(nameof(Recordtransfersbetweenwarehouses));
        }
        #endregion

        public async Task<IActionResult> SupplierCodes()
        {
            return View(new List<CategoryDto>());
        }
        [HttpPost]

        //[HttpPost]
        //public async Task<IActionResult> UpdateUser([FromBody] InvUserDto usersData)
        //{
        //    if (usersData == null)
        //    {
        //        throw new BadRequestException("NoDataReceived");
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
        //        throw new BadRequestException(firstError ?? "InvalidData");
        //    }
        //    await _sysManagement.UpdateInvUserAsync(usersData);
        //    return Json(new
        //    {
        //        success = true,
        //        message = _localizer["UpdateSuccess"]
        //    });
        //}

        [HttpPost]
        public async Task<IActionResult> DeleteRecord(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID is missing" });
            }

            int numericId = int.Parse(id);
            await _invTransService.DeleteInvTransAsync(numericId);

            return Json(new { success = true, message = "تم الحذف بنجاح" });
        }
        //[HttpPost]
        //public async Task<IActionResult> Delete([FromBody] InvUserDto data)
        //{
        //    // 1. Basic check
        //    if (data.UserCode <= 0)
        //    {
        //        throw new BadRequestException("InvalidUserCode"); // Key for localization
        //    }
        //    await _sysManagement.DeleteInvUserAsync((int)data.UserCode);

        //    // 3. Success Response
        //    return Json(new
        //    {
        //        success = true,
        //        message = _localizer["DeleteSuccess"]
        //    });
        //}

        #region Record Return
        public async Task<IActionResult> IncomingItemsReturns()
        {
            var item = await _itemService.GetAllItemAsync();
            var invtrans = await _invTransService.GetTransactionsByTypeAsync(1, 5); // storeCode=1, trType=5 (Incoming Items Returns)
            var supplier = await _supplierService.GetAllSupplierAsync();
            ViewBag.items = item;
            ViewBag.TodayTransactions = invtrans;
            ViewBag.supplier = supplier;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateIncomingItemsReturn(InwardCreationDto model)
        {
            if (model.Items == null || !model.Items.Any())
            {
                return RedirectToAction(nameof(IncomingItemsReturns));
            }
            var generatedTrNum = await _invTransService.CreateInvTransAsync(model);
            return RedirectToAction(nameof(IncomingItemsReturns));
        }
        public async Task<IActionResult> IssuedItemsReturns()
        {
            var item = await _itemService.GetAllItemAsync();
            var invtrans = await _invTransService.GetTransactionsByTypeAsync(1, 6); // storeCode=1, trType=6 (Issued Items Returns)
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var egxEmployees = await _egxEmployeeService.GetAllEgxEmployeeAsync();
            ViewBag.items = item;
            ViewBag.TodayTransactions = invtrans;
            ViewBag.departments = departments;
            ViewBag.egxEmployees = egxEmployees;
            return View(new List<CategoryDto>());
        }

        [HttpPost]
        public async Task<IActionResult> CreateIssuedItemsReturn(InwardCreationDto model)
        {
            if (model.Items == null || !model.Items.Any())
            {
                return RedirectToAction(nameof(IssuedItemsReturns));
            }
            var generatedTrNum = await _invTransService.CreateInvTransAsync(model);
            return RedirectToAction(nameof(IssuedItemsReturns));
        }

        #endregion

        #region Dead Stock entry
        public async Task<IActionResult> DeadStockEntry()
        {
            var item = await _itemService.GetAllItemAsync();
            var invtrans = await _invTransService.GetTransactionsByTypeAsync(1, 7); // storeCode=1, trType=7 (Dead Stock)
            var supplier = await _supplierService.GetAllSupplierAsync();

            ViewBag.supplier = supplier;
            ViewBag.items = item;
            ViewBag.TodayTransactions = invtrans;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDeadStock(InwardCreationDto model)
        {
            if (model.Items == null || !model.Items.Any())
            {
                return RedirectToAction(nameof(DeadStockEntry));
            }
            var generatedTrNum = await _invTransService.CreateInvTransAsync(model);
            return RedirectToAction(nameof(DeadStockEntry));
        }
        #endregion

        #region Search and inquiry

        public async Task<IActionResult> SearchAndInquiry(int type = 1, int page = 1, string fromDate = null, string toDate = null)
        {
            int storeCode = 1;
            int pageSize = 20;

            DateTime start = string.IsNullOrEmpty(fromDate)
                ? DateTime.Now.AddDays(-30).Date
                : DateTime.Parse(fromDate);

            DateTime end = string.IsNullOrEmpty(toDate)
                ? DateTime.Now.Date
                : DateTime.Parse(toDate);

            var items = await _itemService.GetAllItemAsync();
            ViewBag.Items = items;
            var typeList = new List<int> { type };
            var pagedTransactions = await _hInvTransService.GetHistoryTransactionsPaginatedAsync(typeList,
                page,
                pageSize,
                start,
                end
            );

            ViewBag.Transactions = pagedTransactions.Items;
            ViewBag.TotalCount = pagedTransactions.TotalCount;
            ViewBag.CurrentPage = pagedTransactions.PageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)pagedTransactions.TotalCount / pageSize);
            ViewBag.CurrentType = type;

            ViewBag.FromDate = start.ToString("yyyy-MM-dd");
            ViewBag.ToDate = end.ToString("yyyy-MM-dd");

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetTransactionsData(string trType, string fromDate, string toDate, int page = 1, int pageSize = 20)
        {
            try
            {
                var typeList = trType.Split(',').Select(int.Parse).ToList();

                // 2. Safely handle dates
                if (!DateTime.TryParse(fromDate, out DateTime start))
                    start = DateTime.Now.AddDays(-30).Date;

                if (!DateTime.TryParse(toDate, out DateTime end))
                    end = DateTime.Now.Date;

                var result = await _hInvTransService.GetHistoryTransactionsPaginatedAsync(
                    typeList,
                    page,
                    pageSize,
                    start,
                    end
                );

                return Json(new
                {
                    items = result.Items,
                    totalCount = result.TotalCount,
                    pageNumber = result.PageNumber
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error processing request", details = ex.Message });
            }
        }


        #endregion

    }
}
