using Application.Interfaces.Contracts.Localization;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using Application.Interfaces.Models.Pagination;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace InventoryManagement.Controllers
{
    public class MainItemsController : Controller
    {
        private readonly ILogger<MainItemsController> _logger;
        private readonly IAppLocalizer _localizer;
        private readonly IItemCategoryService _itemCategoryService;
        private readonly IItemService _itemService;
        private readonly ISystemManagementService _sysManagement;
        private readonly ISupplierService _supplierService;

        public MainItemsController(ILogger<MainItemsController> logger, IAppLocalizer localizer, IItemCategoryService itemCategoryService,
            IItemService itemService, ISystemManagementService sysManagement, ISupplierService supplierService)
        {
            _logger = logger;
            _localizer = localizer;
            _itemService = itemService;
            _itemCategoryService = itemCategoryService;
            _sysManagement = sysManagement;
            _supplierService = supplierService;
        }
        public async Task<IActionResult> MainCategoryClassification()
        {
            return View(await _itemCategoryService.GetAllItemCategoryAsync());
        }
        public async Task<IActionResult> SubCategoryClassificationcopy()
        {
            ViewBag.Categories = await _itemCategoryService.GetAllItemCategoryAsync();
            return View(await _itemService.GetAllItemAsync());
        }
        public async Task<IActionResult> SupplierCodes()
        {
            return View(await _supplierService.GetAllSupplierAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Create(string CatgryDesc)
        {
            try
            {
                // 1. Basic null check
                if (string.IsNullOrWhiteSpace(CatgryDesc))
                {
                    return Json(new { success = false, message = "يرجى إدخال اسم التصنيف" });
                }

                if (!ModelState.IsValid)
                {
                    var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { success = false, message = firstError ?? "بيانات غير صالحة" });
                }

                var result = await _itemCategoryService.CreateItemCategoryAsync(new ItemCategoryDto() { CatgryDesc = CatgryDesc });

                return Json(new { success = true, message = "تم إضافة البيانات بنجاح" });
            }
            catch (BadRequestException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ غير متوقع: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string CatgryCode, string CatgryDesc)
        {
            await _itemCategoryService.UpdateItemCategoryAsync(new ItemCategoryDto() { CatgryCode = CatgryCode, CatgryDesc = CatgryDesc });
            return Json(new { success = true, message = "تم تحديث البيانات بنجاح" });
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await _itemCategoryService.DeleteItemCategoryAsync(id);
            return Json(new { success = true, message = "تم حذف التصنيف بنجاح" });
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(ItemDto model)
        {
            try
            {
                // 1. Basic null check
                if (string.IsNullOrWhiteSpace(model.ItemDesc))
                {
                    return Json(new { success = false, message = "يرجى إدخال اسم التصنيف" });
                }

                if (!ModelState.IsValid)
                {
                    var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { success = false, message = firstError ?? "بيانات غير صالحة" });
                }

                var result = await _itemService.CreateItemAsync(model);

                return Json(new { success = true, message = "تم إضافة البيانات بنجاح" });
            }
            catch (BadRequestException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ غير متوقع: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Update(ItemDto command, string oldCatgryCode)
        {
            await _itemService.UpdateItemAsync(command, oldCatgryCode);
            return Json(new { success = true, message = "تم تحديث البيانات بنجاح" });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteItem(string id)
        {
            await _itemService.DeleteItemAsync(id);
            return Json(new { success = true, message = "تم حذف التصنيف بنجاح" });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier(SupplierDto model)
        {
            try
            {
                // 1. Basic null check
                if (string.IsNullOrWhiteSpace(model.SuplierDesc))
                {
                    return Json(new { success = false, message = "يرجى إدخال اسم المورد" });
                }

                if (!ModelState.IsValid)
                {
                    var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { success = false, message = firstError ?? "بيانات غير صالحة" });
                }

                var result = await _supplierService.CreateSupplierAsync(model);

                return Json(new { success = true, message = "تم إضافة البيانات بنجاح" });
            }
            catch (BadRequestException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ غير متوقع: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSupplier(SupplierDto command, string oldCatgryCode)
        {
            await _supplierService.UpdateSupplierAsync(command, oldCatgryCode);
            return Json(new { success = true, message = "تم تحديث البيانات بنجاح" });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteSupplier(string id)
        {
            await _supplierService.DeleteSupplierAsync(id);
            return Json(new { success = true, message = "تم حذف المورد بنجاح" });
        }


        public async Task<IActionResult> SubCategoryClassification()
        {
            ViewBag.Categories = await _itemCategoryService.GetAllItemCategoryAsync();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetItemsTable(int page = 1, string search = "", string category = "")
        {
            var model = await _itemService.GetItemsPaginated(page, search, category);
            return PartialView("_ItemsTableList", model);
        }
        [HttpGet]
        public async Task<IActionResult> GetSupplierTable(int page = 1, string search = "", string category = "")
        {
            var result = await _supplierService.GetSupplierPaginated(page, search, category);

            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalCount = result.TotalCount;
            return PartialView("_SuppliersTableList", result.Items);
        }
    }
}
