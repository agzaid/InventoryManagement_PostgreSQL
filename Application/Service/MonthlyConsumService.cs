using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using FluentValidation;

namespace Application.Service
{
    internal class MonthlyConsumService : IMonthlyConsumService
    {
        private readonly IValidator<MonthlyConsumDto> _validator;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MonthlyConsumService(IValidator<MonthlyConsumDto> validator, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<MonthlyConsumDto>> GetAllMonthlyConsumsAsync()
        {
            var items = await _unitOfWork.MonthlyConsumRepository.GetAllAsync();
            return _mapper.Map<IReadOnlyList<MonthlyConsumDto>>(items);
        }

        public async Task<IReadOnlyList<MonthlyConsumDto>> GetMonthlyConsumsByStoreAsync(int storeCode)
        {
            var items = await _unitOfWork.MonthlyConsumRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == storeCode,
                orderBy: x => x.ItemCode
            );
            return _mapper.Map<IReadOnlyList<MonthlyConsumDto>>(items);
        }

        public async Task<IReadOnlyList<MonthlyConsumDto>> GetMonthlyConsumsByYearMonthAsync(int storeCode, int year, int month)
        {
            var items = await _unitOfWork.MonthlyConsumRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == storeCode && x.ConsumYear == year && x.ConsumMonth == month,
                orderBy: x => x.ItemCode
            );
            return _mapper.Map<IReadOnlyList<MonthlyConsumDto>>(items);
        }

        public async Task<MonthlyConsumDto?> GetMonthlyConsumByIdAsync(int storeCode, int year, int month, int depCode, string itemCode)
        {
            var items = await _unitOfWork.MonthlyConsumRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == storeCode && x.ConsumYear == year && x.ConsumMonth == month 
                          && x.DepCode == depCode && x.ItemCode == itemCode,
                orderBy: x => x.ItemCode
            );
            var item = items.FirstOrDefault();
            if (item == null)
                return null;
            return _mapper.Map<MonthlyConsumDto>(item);
        }

        public async Task<int> CreateMonthlyConsumAsync(MonthlyConsumDto command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var entity = _mapper.Map<MonthlyConsum>(command);
            await _unitOfWork.MonthlyConsumRepository.AddAsync(entity);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("لم يتم حفظ البيانات في قاعدة البيانات.");
            }

            return result;
        }

        public async Task UpdateMonthlyConsumAsync(MonthlyConsumDto command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var items = await _unitOfWork.MonthlyConsumRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == command.StoreCode && x.ConsumYear == command.ConsumYear 
                          && x.ConsumMonth == command.ConsumMonth && x.DepCode == command.DepCode 
                          && x.ItemCode == command.ItemCode,
                orderBy: x => x.ItemCode
            );
            var existingItem = items.FirstOrDefault();

            if (existingItem == null)
            {
                throw new NotFoundException($"سجل الاستهلاك الشهري غير موجود.");
            }

            _mapper.Map(command, existingItem);
            await _unitOfWork.MonthlyConsumRepository.UpdateAsync(existingItem);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("لم يتم إجراء أي تغييرات على البيانات.");
            }
        }

        public async Task DeleteMonthlyConsumAsync(int storeCode, int year, int month, int depCode, string itemCode)
        {
            var items = await _unitOfWork.MonthlyConsumRepository.GetAllAsyncExpression(
                filter: x => x.StoreCode == storeCode && x.ConsumYear == year && x.ConsumMonth == month 
                          && x.DepCode == depCode && x.ItemCode == itemCode,
                orderBy: x => x.ItemCode
            );
            var item = items.FirstOrDefault();

            if (item == null)
            {
                throw new NotFoundException($"سجل الاستهلاك الشهري غير موجود.");
            }

            await _unitOfWork.MonthlyConsumRepository.DeleteAsync(item);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception("حدث خطأ أثناء محاولة حذف البيانات من قاعدة البيانات.");
            }
        }
    }
}
