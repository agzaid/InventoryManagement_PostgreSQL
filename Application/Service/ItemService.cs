using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using Application.Interfaces.Models.Pagination;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using FluentValidation;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Application.Service
{
    internal class ItemService : IItemService
    {
        private readonly IValidator<ItemDto> _validator;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ItemService(IValidator<ItemDto> validator, IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> CreateItemAsync(ItemDto command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            var isNameUnique = await _unitOfWork.ItemRepository.GetByNameAsync(command.ItemDesc);

            if (isNameUnique != null)
            {
                throw new BadRequestException($"Item name '{command.ItemDesc}' is already in use.");
            }

            // 2. بدء المعاملة (Transaction) لضمان "الكل أو لا شيء"
            await using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var item = _mapper.Map<Item>(command);
                    item.ItemCategory = null;

                    // حفظ الصنف أولاً للحصول على الـ ItemCode
                    await _unitOfWork.ItemRepository.AddAsync(item);
                    await _unitOfWork.SaveChangesAsync();

                    // إضافة الرصيد الافتتاحي باستخدام الكود الناتج
                    var openBal = new OpenBalance()
                    {
                        ItemCode = item.ItemCode,
                        OpenDate = DateTime.Now.Date,
                        StoreCode = 1,
                        OpenBal = 0,
                        RelayFlag = "0"
                    };

                    await _unitOfWork.OpenBalanceRepository.AddAsync(openBal);
                    await _unitOfWork.SaveChangesAsync();

                    // تثبيت كافة التغييرات نهائياً
                    await transaction.CommitAsync();

                    return item.ItemCode;
                }
                catch (Exception ex)
                {
                    // في حال حدوث أي خطأ، يتم إلغاء كل ما تم حفظه (حتى الـ Item)
                    await transaction.RollbackAsync();

                    // إعادة رمي الخطأ ليتم التعامل معه في الـ Controller أو Middleware
                    throw new NotFoundException("فشلت عملية إنشاء الصنف والرصيد الافتتاحي: " + ex.Message);
                }
            }
        }

        public async Task<IReadOnlyList<ItemDto>> GetAllItemAsync()
        {

            var itemCategories = await _unitOfWork.ItemRepository.GetAllItemsWithCategory();

            return _mapper.Map<IReadOnlyList<ItemDto>>(itemCategories);
        }

        public Task<ItemDto> GetItemByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateItemAsync(ItemDto command, string oldCatgryCode)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            var categoryWithSameName = await _unitOfWork.ItemRepository.GetByNameAsync(command.ItemDesc);
            if (categoryWithSameName != null && categoryWithSameName.CatgryCode != command.CatgryCode)
            {
                throw new BadRequestException($"Item name '{command.ItemDesc}' is already in use by another Item.");
            }
            var existingItem = await _unitOfWork.ItemRepository.GetByIdStringAsync(command.ItemCode);
            if (existingItem==null)
            {
                throw new BadRequestException($"Item name '{command.ItemDesc}' is not there");
            }

            var itemEdited = _mapper.Map<Item>(command);
            itemEdited.ItemCategory = null;

            if (oldCatgryCode != command.CatgryCode)
            {
                // حذف القديم
                await _unitOfWork.ItemRepository.DeleteAsync(existingItem);

                // إضافة الجديد (بنفس ItemCode ولكن CatgryCode جديد)
                var newItem = _mapper.Map<Item>(command);
                newItem.ItemCategory = null;
                newItem.CatgryCode = oldCatgryCode;
                await _unitOfWork.ItemRepository.AddAsync(newItem);
            }
            else
            {
                // 3. تحديث عادي إذا لم يتغير التصنيف
                _mapper.Map(command, existingItem);
                existingItem.ItemCategory = null;
                await _unitOfWork.ItemRepository.UpdateAsync(existingItem);
            }

           var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception("حدث خطأ أثناء محاولة حذف البيانات من قاعدة البيانات.");
            }
        }
        public async Task DeleteItemAsync(string itemcode)
        {
            // 1. Validate Input
            if (string.IsNullOrWhiteSpace(itemcode))
            {
                throw new BadRequestException("Category code is required for deletion.");
            }

            // 2. Fetch the existing entity
            var itemCategory = await _unitOfWork.ItemRepository.GetByIdStringAsync(itemcode);

            if (itemCategory == null)
            {
                throw new NotFoundException($"Item Category with code '{itemcode}' was not found.");
            }
            await _unitOfWork.ItemRepository.DeleteAsync(itemCategory);

            // 5. Commit changes
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception("حدث خطأ أثناء محاولة حذف البيانات من قاعدة البيانات.");
            }
        }

        public async Task<PagedResult<ItemDto>> GetItemsPaginated(int page, string search, string category)
        {
            int pageSize = 20;

            // 1. تنظيف النص العربي من المسافات الزائدة
            string cleanSearch = search?.Trim() ?? "";

            // 2. بناء الفلتر
            Expression<Func<Item, bool>> filter = x =>
                (string.IsNullOrEmpty(cleanSearch) ||
                 x.ItemDesc.Contains(cleanSearch) ||
                 x.ItemCode.Contains(cleanSearch)) &&
                (string.IsNullOrEmpty(category) ||
                 (x.ItemCategory != null && x.ItemCategory.CatgryDesc == category));

            // 3. التنفيذ
            var pagedResult = await _unitOfWork.ItemRepository.GetPagedAsync(
                page,
                pageSize,
                filter: filter,
                orderBy: x => x.ItemCode,
                includeProperties: "ItemCategory"
            );

            // إذا كانت النتيجة null، نرجع كائن فارغ بدلاً من الخطأ
            if (pagedResult == null || pagedResult.Items == null)
            {
                return new PagedResult<ItemDto> { Items = new List<ItemDto>(), TotalCount = 0 };
            }

            return new PagedResult<ItemDto>
            {
                Items = _mapper.Map<IReadOnlyList<ItemDto>>(pagedResult.Items),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<int> GetTotalCountAsync()
        {
            var items = await _unitOfWork.ItemRepository.GetAllAsync();
            return items.Count;
        }
    }
}
