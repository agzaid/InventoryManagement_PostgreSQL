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

            await using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var item = _mapper.Map<Item>(command);
                    item.ItemCategory = null;

                    if (string.IsNullOrEmpty(item.ItemCode))
                    {
                        var allItems = await _unitOfWork.ItemRepository.GetAllAsync();
                        int nextCode = allItems.Count > 0 ? 
                            int.Parse(allItems.Max(i => i.ItemCode ?? "0")) + 1 : 1;
                        item.ItemCode = nextCode.ToString().PadLeft(5, '0');
                    }

                    await _unitOfWork.ItemRepository.AddAsync(item);
                    await _unitOfWork.SaveChangesAsync();

                    var openBal = new OpenBalance()
                    {
                        ItemCode = item.ItemCode,
                        OpenDate = DateTime.UtcNow.Date,
                        StoreCode = 1,
                        OpenBal = item.RecallQnt ?? 0,
                        RelayFlag = "0"
                    };

                    await _unitOfWork.OpenBalanceRepository.AddAsync(openBal);
                    await _unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return item.ItemCode;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

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

        public async Task UpdateItemAsync(ItemDto command, int oldCatgryCode)
        {
            // 1. Validation
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

            // 2. Fetch existing item (Tracked by EF)
            var existingItem = await _unitOfWork.ItemRepository.GetByIdStringAsync(command.ItemCode);
            if (existingItem == null) throw new BadRequestException($"Item {command.ItemCode} not found");

            command.Id = existingItem.Id;
            _mapper.Map(command, existingItem);
            existingItem.ItemCategory = null;

            // Explicitly update the category if it changed
            existingItem.CatgryCode = command.CatgryCode ?? 0;

            await _unitOfWork.ItemRepository.UpdateAsync(existingItem);

            // 4. Update the OpenBalance (Opening Quantity)
            var openBalRecord = await _unitOfWork.OpenBalanceRepository.GetByIdStringAsync(command.ItemCode);
            if (openBalRecord != null)
            {
                openBalRecord.OpenBal = command.RecallQnt ?? 0;
                // Remember the PostgreSQL UTC rule from earlier!
                openBalRecord.OpenDate = DateTime.UtcNow;
                await _unitOfWork.OpenBalanceRepository.UpdateAsync(openBalRecord);
            }
            else if (command.RecallQnt > 0)
            {
                // Create it if it didn't exist
                var newBal = new OpenBalance
                {
                    ItemCode = command.ItemCode,
                    OpenBal = command.RecallQnt ?? 0,
                    OpenDate = DateTime.UtcNow,
                    StoreCode = 1
                };
                await _unitOfWork.OpenBalanceRepository.AddAsync(newBal);
            }

            // 5. Save all changes at once
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
                throw new Exception("No changes were saved to the database.");
        }
        public async Task DeleteItemAsync(string itemcode)
        {
            if (string.IsNullOrWhiteSpace(itemcode))
            {
                throw new BadRequestException("Category code is required for deletion.");
            }
            var itemCategory = await _unitOfWork.ItemRepository.GetByIdStringAsync(itemcode);

            if (itemCategory == null)
            {
                throw new NotFoundException($"Item Category with code '{itemcode}' was not found.");
            }
            await _unitOfWork.ItemRepository.DeleteAsync(itemCategory);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception("حدث خطأ أثناء محاولة حذف البيانات من قاعدة البيانات.");
            }
        }

        public async Task<PagedResult<ItemDto>> GetItemsPaginated(int page, string search, string category)
        {
            int pageSize = 20;

            string cleanSearch = search?.Trim() ?? "";

            Expression<Func<Item, bool>> filter = x =>
                (string.IsNullOrEmpty(cleanSearch) ||
                 x.ItemDesc.Contains(cleanSearch) ||
                 x.ItemCode.Contains(cleanSearch)) &&
                (string.IsNullOrEmpty(category) ||
                 (x.ItemCategory != null && x.ItemCategory.CatgryDesc == category));

            var pagedResult = await _unitOfWork.ItemRepository.GetPagedAsync(
                page,
                pageSize,
                filter: filter,
                orderBy: x => x.ItemCode,
                includeProperties: "ItemCategory"
            );

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
