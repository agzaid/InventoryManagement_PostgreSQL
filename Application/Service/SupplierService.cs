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
    internal class SupplierService : ISupplierService
    {
        private readonly IValidator<SupplierDto> _validator;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SupplierService(IValidator<SupplierDto> validator, IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateSupplierAsync(SupplierDto command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            var isNameUnique = await _unitOfWork.SupplierRepository.GetByNameAsync(command.SuplierDesc);

            if (isNameUnique != null)
            {
                throw new BadRequestException($"Item name '{command.SuplierDesc}' is already in use.");
            }

            var item = _mapper.Map<Supplier>(command);

            await _unitOfWork.SupplierRepository.AddAsync(item);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception("حدث خطأ أثناء محاولة حذف البيانات من قاعدة البيانات.");
            }
            return item.SuplierCode;
        }

        public async Task DeleteSupplierAsync(string id)
        {
            // 1. Validate Input
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new BadRequestException("Category code is required for deletion.");
            }

            // 2. Fetch the existing entity
            var itemCategory = await _unitOfWork.SupplierRepository.GetByIdStringAsync(id);

            if (itemCategory == null)
            {
                throw new NotFoundException($"Supplier with code '{id}' was not found.");
            }
            await _unitOfWork.SupplierRepository.DeleteAsync(itemCategory);

            // 5. Commit changes
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception("حدث خطأ أثناء محاولة حذف البيانات من قاعدة البيانات.");
            }
        }

        public async Task<IReadOnlyList<SupplierDto>> GetAllSupplierAsync()
        {
            var itemCategories = await _unitOfWork.SupplierRepository.GetAllAsync();

            return _mapper.Map<IReadOnlyList<SupplierDto>>(itemCategories);
        }
        public Task<SupplierDto> GetSupplierByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<SupplierDto>> GetSupplierPaginated(int page, string search, string category)
        {
            int pageSize = 20;

            // 1. تنظيف النص العربي من المسافات الزائدة
            string cleanSearch = search?.Trim() ?? "";

            // 2. بناء الفلتر
            Expression<Func<Supplier, bool>> filter = x =>
                (string.IsNullOrEmpty(cleanSearch) ||
                 x.SuplierDesc.Contains(cleanSearch)) &&
                (string.IsNullOrEmpty(category));

            // 3. التنفيذ
            var pagedResult = await _unitOfWork.SupplierRepository.GetPagedAsync(
                page,
                pageSize,
                filter: filter,
                orderBy: x => x.SuplierCode,
                includeProperties: ""
            );

            // إذا كانت النتيجة null، نرجع كائن فارغ بدلاً من الخطأ
            if (pagedResult == null || pagedResult.Items == null)
            {
                return new PagedResult<SupplierDto> { Items = new List<SupplierDto>(), TotalCount = 0 };
            }

            return new PagedResult<SupplierDto>
            {
                Items = _mapper.Map<IReadOnlyList<SupplierDto>>(pagedResult.Items),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task UpdateSupplierAsync(SupplierDto command, string oldCategory)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            var existingItem = await _unitOfWork.SupplierRepository.GetByIdStringAsync(command.SuplierCode.ToString());
            if (existingItem == null)
            {
                throw new BadRequestException($"Item name '{command.SuplierCode}' is not there");
            }

            var itemEdited = _mapper.Map(command, existingItem);

            await _unitOfWork.SupplierRepository.UpdateAsync(itemEdited);

            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception("حدث خطأ أثناء محاولة حذف البيانات من قاعدة البيانات.");
            }
        }

        public async Task<int> GetTotalCountAsync()
        {
            var items = await _unitOfWork.SupplierRepository.GetAllAsync();
            return items.Count;
        }
    }
}
