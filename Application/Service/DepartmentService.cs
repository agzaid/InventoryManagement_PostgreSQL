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
    internal class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IValidator<DepartmentDto> _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DepartmentService(
            IDepartmentRepository repository, IValidator<DepartmentDto> validator,
            IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<int> CreateDepartmentAsync(DepartmentDto command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

            var isNameUnique = await _repository.GetByNameAsync(command.DepDesc);

            if (isNameUnique != null)
            {
                throw new BadRequestException($"Department name '{command.DepDesc}' is already in use.");
            }

            // 2. Map the Application DTO to the Domain Entity
            // var department = _mapper.Map<Department>(command);

            // Temporary manual mapping:
            var department = new Department
            {
                DepDesc = command.DepDesc
                // Note: Id is usually set by the database
            };

            // 3. Add the entity to the context (tracked state)
            await _repository.AddAsync(department);

            // 4. Commit the changes to the database

           var result = await _unitOfWork.SaveChangesAsync();

            // 5. Return the newly created ID
            return department.DepCode;
        }

        public Task DeleteDepartmentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<DepartmentDto>> GetAllDepartmentsAsync()
        {
            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();

            return _mapper.Map<IReadOnlyList<DepartmentDto>>(departments);
        }

        public Task<DepartmentDto> GetDepartmentByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDepartmentAsync(DepartmentDto command)
        {
        //Update: _validator.ValidateAsync(command, options => options.IncludeRuleSets("Update"));
            throw new NotImplementedException();
        }
    }
}
