using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using FluentValidation;
using System.Collections.Generic;
using System.Text;

namespace Application.Service
{
    internal class ItemCategoryService : IItemCategoryService
    {
        private readonly IValidator<ItemCategoryDto> _validator;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ItemCategoryService(IValidator<ItemCategoryDto> validator, IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> CreateItemCategoryAsync(ItemCategoryDto command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new FluentValidation.ValidationException(validationResult.Errors);
            }
            var isNameUnique = await _unitOfWork.ItemCategoryRepository.GetByNameAsync(command.CatgryDesc.Trim());

            if (isNameUnique != null)
            {
                throw new BadRequestException($"Item Category name '{command.CatgryDesc}' is already in use.");
            }

            var allCategories = await _unitOfWork.ItemCategoryRepository.GetAllAsync();
            int nextId = allCategories.Count > 0 ? allCategories.Max(c => c.Id) + 1 : 1;
            string nextCatgryCode = nextId.ToString().PadLeft(2, '0');

            var itemCategory = new ItemCategory
            {
                CatgryCode = nextCatgryCode, // Set CatgryCode before adding
                CatgryDesc = command.CatgryDesc
            };
            
            // Add and Save
            await _unitOfWork.ItemCategoryRepository.AddAsync(itemCategory);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("حدث خطأ أثناء محاولة إضافة البيانات إلى قاعدة البيانات.");
            }
            return itemCategory.CatgryCode;
        }

        public async Task<IReadOnlyList<ItemCategoryDto>> GetAllItemCategoryAsync()
        {

            var itemCategories = await _unitOfWork.ItemCategoryRepository.GetAllAsync();

            return _mapper.Map<IReadOnlyList<ItemCategoryDto>>(itemCategories);
        }

        public Task<ItemCategoryDto> GetItemCategoryByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateItemCategoryAsync(ItemCategoryDto command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Fetch the existing entity from the database
            // Ensure you use the unique identifier (CatgryCode)
            var existingCategory = await _unitOfWork.ItemCategoryRepository.GetByIdStringAsync(command.CatgryCode);

            if (existingCategory == null)
            {
                throw new NotFoundException($"Item Category with code '{command.CatgryCode}' not found.");
            }
            var categoryWithSameName = await _unitOfWork.ItemCategoryRepository.GetByNameAsync(command.CatgryDesc);
            if (categoryWithSameName != null && categoryWithSameName.CatgryCode != command.CatgryCode)
            {
                throw new BadRequestException($"Item Category name '{command.CatgryDesc}' is already in use by another category.");
            }
            existingCategory.CatgryDesc = command.CatgryDesc;
            await _unitOfWork.ItemCategoryRepository.UpdateAsync(existingCategory);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception("حدث خطأ أثناء محاولة حذف البيانات من قاعدة البيانات.");
            }
        }
        public async Task DeleteItemCategoryAsync(string catgryCode)
        {
            // 1. Validate Input
            if (string.IsNullOrWhiteSpace(catgryCode))
            {
                throw new BadRequestException("Category code is required for deletion.");
            }

            // 2. Fetch the existing entity
            var itemCategory = await _unitOfWork.ItemCategoryRepository.GetByIdStringAsync(catgryCode);

            if (itemCategory == null)
            {
                throw new NotFoundException($"Item Category with code '{catgryCode}' was not found.");
            }

            // 3. Business Rule Check (Recommended)
            // Check if there are any items/products linked to this category before deleting
            // var hasRelatedItems = await _unitOfWork.ItemsRepository.AnyAsync(x => x.CatgryCode == catgryCode);
            // if (hasRelatedItems)
            // {
            //     throw new BadRequestException("لا يمكن حذف هذا التصنيف لأنه مرتبط بمنتجات موجودة بالفعل.");
            // }

            // 4. Perform the deletion
            await _unitOfWork.ItemCategoryRepository.DeleteAsync(itemCategory);

            // 5. Commit changes
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception("حدث خطأ أثناء محاولة حذف البيانات من قاعدة البيانات.");
            }
        }

        public async Task<int> GetTotalCountAsync()
        {
            var items = await _unitOfWork.ItemCategoryRepository.GetAllAsync();
            return items.Count;
        }

        public async Task<IReadOnlyList<CategoryDistributionDto>> GetCategoryDistributionAsync()
        {
            var categories = await _unitOfWork.ItemCategoryRepository.GetAllAsync();
            var items = await _unitOfWork.ItemRepository.GetAllAsync();

            int totalItems = items.Count;

            var distribution = categories.Select(c =>
            {
                int count = items.Count(i => i.CatgryCode == c.CatgryCode);
                return new CategoryDistributionDto
                {
                    CategoryCode = c.CatgryCode,
                    CategoryName = c.CatgryDesc,
                    ItemCount = count,
                    Percentage = totalItems > 0 ? Math.Round((decimal)count / totalItems * 100, 1) : 0
                };
            })
            .OrderByDescending(x => x.ItemCount)
            .ToList();

            return distribution;
        }
    }
}
