using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using FluentValidation;

namespace Application.Service
{
    internal class ItemBalanceService : IItemBalanceService
    {
        private readonly IValidator<ItemBalanceDto> _validator;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ItemBalanceService(IValidator<ItemBalanceDto> validator, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<ItemBalanceDto>> GetAllItemBalancesAsync()
        {
            var items = await _unitOfWork.ItemBalanceRepository.GetAllAsync();
            return _mapper.Map<IReadOnlyList<ItemBalanceDto>>(items);
        }

        public async Task<IReadOnlyList<ItemBalanceDto>> GetItemBalancesByStoreAsync(int storeCode)
        {
            var items = await _unitOfWork.ItemBalanceRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == storeCode,
                orderBy: x => x.ItemCode
            );
            return _mapper.Map<IReadOnlyList<ItemBalanceDto>>(items);
        }

        public async Task<ItemBalanceDto?> GetItemBalanceByIdAsync(int storeCode, string itemCode)
        {
            var items = await _unitOfWork.ItemBalanceRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == storeCode && x.ItemCode == itemCode,
                orderBy: x => x.ItemCode
            );
            var item = items.FirstOrDefault();
            if (item == null)
                return null;
            return _mapper.Map<ItemBalanceDto>(item);
        }

        public async Task<int> CreateItemBalanceAsync(ItemBalanceDto command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var entity = _mapper.Map<ItemBalance>(command);
            await _unitOfWork.ItemBalanceRepository.AddAsync(entity);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("لم يتم حفظ البيانات في قاعدة البيانات.");
            }

            return result;
        }

        public async Task UpdateItemBalanceAsync(ItemBalanceDto command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var items = await _unitOfWork.ItemBalanceRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == command.StoreCode && x.ItemCode == command.ItemCode,
                orderBy: x => x.ItemCode
            );
            var existingItem = items.FirstOrDefault();

            if (existingItem == null)
            {
                throw new NotFoundException($"رصيد الصنف '{command.ItemCode}' غير موجود.");
            }

            _mapper.Map(command, existingItem);
            await _unitOfWork.ItemBalanceRepository.UpdateAsync(existingItem);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("لم يتم إجراء أي تغييرات على البيانات.");
            }
        }

        public async Task DeleteItemBalanceAsync(int storeCode, string itemCode)
        {
            var items = await _unitOfWork.ItemBalanceRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == storeCode && x.ItemCode == itemCode,
                orderBy: x => x.ItemCode
            );
            var item = items.FirstOrDefault();

            if (item == null)
            {
                throw new NotFoundException($"رصيد الصنف '{itemCode}' غير موجود.");
            }

            await _unitOfWork.ItemBalanceRepository.DeleteAsync(item);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception("حدث خطأ أثناء محاولة حذف البيانات من قاعدة البيانات.");
            }
        }

        public async Task<CurrentStockDto> GetCurrentStockSummaryAsync(int storeCode, string? itemCode = null)
        {
            var balanceList = await _unitOfWork.ItemBalanceRepository.GetAllAsyncExpression(
                filter: b => b.ItemCode == itemCode && b.StoreCode == storeCode,
                orderBy: s => s.BalDate,
                descending: true,
                tracked: false
            );


            var currentBalanceRecord = balanceList.FirstOrDefault();

            if (currentBalanceRecord == null)
            {
                return new CurrentStockDto();
            }

            // Get latest balance per item
            var latestBalances = balanceList
                .GroupBy(b => b.ItemCode)
                .Select(g => g.First())
                .ToList();
            var secondLatestBalances = balanceList
                       .GroupBy(b => b.ItemCode)
                       .Select(g => g
                           .OrderByDescending(b => b.BalDate) // Replace with your actual Date property
                           .Skip(1)                               // Skip the latest
                           .FirstOrDefault()                      // Take the second latest (returns null if only 1 record exists)
                       )
                       .Where(b => b != null)                     // Remove nulls if an item only had one record
                       .ToList();
                   
            // Previous Balance data (from ItemBalance table - رصيد اليوم السابق)
            decimal totalStockBalance = latestBalances.Sum(b => b.CurrentBal);
            decimal prevInward = latestBalances.Sum(b => b.ItemIn);
            decimal prevOutward = latestBalances.Sum(b => b.ItemOut);
            decimal prevTransFrom = latestBalances.Sum(b => b.ItemFrom);
            decimal prevTransTo = latestBalances.Sum(b => b.ItemTo);
            decimal openingBalance = secondLatestBalances.Any()
                                     ? secondLatestBalances.FirstOrDefault().OpenBal
                                     : latestBalances.Sum(b => b.OpenBal);
            var latestBalance = latestBalances.FirstOrDefault();
            var secondBalance = secondLatestBalances.FirstOrDefault();

            return new CurrentStockDto
            {
                // Today's Movement (حركة اليوم) - from latest balance record
                TodayInward = latestBalance?.ItemIn ?? 0,
                TodayOutward = latestBalance?.ItemOut ?? 0,
                TodayTransFrom = latestBalance?.ItemFrom ?? 0,
                TodayTransTo = latestBalance?.ItemTo ?? 0,
                TodayReturnIn = latestBalance?.ItemBack ?? 0,
                TodayReturnOut = latestBalance?.ItemBack2 ?? 0,
                TodayStagnant = latestBalance?.ItemScrap ?? 0,
                TodayBalance = latestBalance?.CurrentBal ?? 0,

                // Second Balance (رصيد اليوم السابق) - from second latest balance record
                SecondInward = secondBalance?.ItemIn ?? 0,
                SecondOutward = secondBalance?.ItemOut ?? 0,
                SecondTransFrom = secondBalance?.ItemFrom ?? 0,
                SecondTransTo = secondBalance?.ItemTo ?? 0,
                SecondReturnIn = secondBalance?.ItemBack ?? 0,
                SecondReturnOut = secondBalance?.ItemBack2 ?? 0,
                SecondStagnant = secondBalance?.ItemScrap ?? 0,
                SecondBalance = secondBalance?.CurrentBal ?? 0,

                OpeningBalance = openingBalance,
                PrevInward = prevInward,
                PrevOutward = prevOutward,
                PrevTransFrom = prevTransFrom,
                PrevTransTo = prevTransTo,
                PrevReturnIn = 0,
                PrevReturnOut = 0,
                PrevStagnant = 0,
                TotalStockBalance = totalStockBalance
            };
        }

        public async Task<IReadOnlyList<LowStockItemDto>> GetLowStockItemsAsync(int storeCode, decimal threshold = 20, int? limit = null)
        {
           
            var balanceList = await _unitOfWork.ItemBalanceRepository.GetAllAsyncExpression(
                filter: s => s.StoreCode == storeCode,
                orderBy: s => s.BalDate,
                descending: true,
                tracked: false
            );

            var latestBalances = balanceList
                .GroupBy(b => b.ItemCode)
                .Select(g => g.First())
                .Where(b => b.CurrentBal < threshold)
                .OrderBy(b => b.CurrentBal)
                .ToList();

            if (limit.HasValue)
            {
                latestBalances = latestBalances.Take(limit.Value).ToList();
            }

            var items = await _unitOfWork.ItemRepository.GetAllItemsWithCategory();
            var itemDict = items.ToDictionary(i => i.ItemCode, i => i);

            var result = latestBalances.Select(b =>
            {
                itemDict.TryGetValue(b.ItemCode, out var item);
                return new LowStockItemDto
                {
                    ItemCode = b.ItemCode,
                    ItemDesc = item?.ItemDesc ?? "Unknown",
                    CategoryName = item?.ItemCategory?.CatgryDesc ?? "Uncategorized",
                    OpenBalance = b.OpenBal,
                    CurrentBalance = b.CurrentBal,
                    ReorderQuantity = item?.RecallQnt,
                    LastMovementDate = b.BalDate
                };
            }).ToList();

            return result;
        }

        public async Task<int> GetLowStockCountAsync(int storeCode, decimal threshold = 20)
        {
            var balanceList = await _unitOfWork.ItemBalanceRepository.GetAllAsyncExpression(
                filter: s => s.StoreCode == storeCode,
                orderBy: s => s.BalDate,
                descending: true,
                tracked: false
            );

            var lowStockCount = balanceList
                .GroupBy(b => b.ItemCode)
                .Select(g => g.First())
                .Count(b => b.CurrentBal < threshold);

            return lowStockCount;
        }

        public async Task<decimal> GetTotalCurrentBalanceAsync(int storeCode)
        {
            var balanceList = await _unitOfWork.ItemBalanceRepository.GetAllAsyncExpression(
                filter: s => s.StoreCode == storeCode,
                orderBy: s => s.BalDate,
                descending: true,
                tracked: false
            );

            var totalBalance = balanceList
                .GroupBy(b => b.ItemCode)
                .Select(g => g.First())
                .Sum(b => b.CurrentBal);

            return totalBalance;
        }
    }
}
