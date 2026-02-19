using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Application.Service
{
    internal class MonthlyBalanceService : IMonthlyBalanceService
    {
        private readonly IValidator<MonthlyBalanceDto> _validator;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MonthlyBalanceService(IValidator<MonthlyBalanceDto> validator, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<MonthlyBalanceDto>> GetAllMonthlyBalancesAsync()
        {
            var items = await _unitOfWork.MonthlyBalanceRepository.GetAllAsync();
            return _mapper.Map<IReadOnlyList<MonthlyBalanceDto>>(items);
        }

        public async Task<IReadOnlyList<MonthlyBalanceDto>> GetMonthlyBalancesByStoreAsync(int storeCode)
        {
            var items = await _unitOfWork.MonthlyBalanceRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == storeCode,
                orderBy: x => x.ItemCode
            );
            return _mapper.Map<IReadOnlyList<MonthlyBalanceDto>>(items);
        }

        public async Task<IReadOnlyList<MonthlyBalanceDto>> GetMonthlyBalancesByYearMonthAsync(int storeCode, int year, int month)
        {
            var items = await _unitOfWork.MonthlyBalanceRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == storeCode && x.BalYear == year && x.BalMonth == month,
                orderBy: x => x.ItemCode
            );
            return _mapper.Map<IReadOnlyList<MonthlyBalanceDto>>(items);
        }

        public async Task<MonthlyBalanceDto?> GetMonthlyBalanceByIdAsync(int storeCode, int year, int month, string itemCode)
        {
            var items = await _unitOfWork.MonthlyBalanceRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == storeCode && x.BalYear == year && x.BalMonth == month && x.ItemCode == itemCode,
                orderBy: x => x.ItemCode
            );
            var item = items.FirstOrDefault();
            if (item == null)
                return null;
            return _mapper.Map<MonthlyBalanceDto>(item);
        }

        public async Task<MonthlyConsumptionSummaryDto> GetMonthlyConsumptionSummaryAsync(int storeCode, int year, int month, int top = 10)
        {
            var items = await _unitOfWork.MonthlyBalanceRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == storeCode && x.BalYear == year && x.BalMonth == month,
                orderBy: x => x.ItemCode,
                tracked: false
            );

            var itemCodes = items.Select(x => x.ItemCode).Distinct().ToList();
            var itemEntities = itemCodes.Count == 0
                ? new List<Item>()
                : (await _unitOfWork.ItemRepository.GetAllAsyncExpression(
                    filter: it => itemCodes.Contains(it.ItemCode),
                    orderBy: it => it.ItemCode,
                    tracked: false
                )).ToList();

            var itemDescByCode = itemEntities
                .GroupBy(x => x.ItemCode)
                .ToDictionary(g => g.Key, g => g.First().ItemDesc);

            decimal totalConsumption = items.Sum(x => x.ItemOut);

            var topItems = items
                .Select(x => new MonthlyConsumptionItemDto
                {
                    ItemCode = x.ItemCode,
                    ItemDesc = itemDescByCode.TryGetValue(x.ItemCode, out var desc) ? desc : x.ItemCode,
                    ConsumptionQnt = x.ItemOut
                })
                .OrderByDescending(x => x.ConsumptionQnt)
                .ThenBy(x => x.ItemCode)
                .Take(top)
                .ToList();

            return new MonthlyConsumptionSummaryDto
            {
                StoreCode = storeCode,
                Year = year,
                Month = month,
                TotalConsumptionQnt = totalConsumption,
                ItemsCount = items.Count,
                TopItems = topItems
            };
        }

        public async Task<int> CreateMonthlyBalanceAsync(MonthlyBalanceDto command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var entity = _mapper.Map<MonthlyBalance>(command);
            await _unitOfWork.MonthlyBalanceRepository.AddAsync(entity);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("لم يتم حفظ البيانات في قاعدة البيانات.");
            }

            return result;
        }

        public async Task UpdateMonthlyBalanceAsync(MonthlyBalanceDto command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var items = await _unitOfWork.MonthlyBalanceRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == command.StoreCode && x.BalYear == command.BalYear 
                          && x.BalMonth == command.BalMonth && x.ItemCode == command.ItemCode,
                orderBy: x => x.ItemCode
            );
            var existingItem = items.FirstOrDefault();

            if (existingItem == null)
            {
                throw new NotFoundException($"سجل الرصيد الشهري غير موجود.");
            }

            _mapper.Map(command, existingItem);
            await _unitOfWork.MonthlyBalanceRepository.UpdateAsync(existingItem);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("لم يتم إجراء أي تغييرات على البيانات.");
            }
        }

        public async Task DeleteMonthlyBalanceAsync(int storeCode, int year, int month, string itemCode)
        {
            var items = await _unitOfWork.MonthlyBalanceRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == storeCode && x.BalYear == year && x.BalMonth == month && x.ItemCode == itemCode,
                orderBy: x => x.ItemCode
            );
            var item = items.FirstOrDefault();

            if (item == null)
            {
                throw new NotFoundException($"سجل الرصيد الشهري غير موجود.");
            }

            await _unitOfWork.MonthlyBalanceRepository.DeleteAsync(item);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception("حدث خطأ أثناء محاولة حذف البيانات من قاعدة البيانات.");
            }
        }
    }
}
