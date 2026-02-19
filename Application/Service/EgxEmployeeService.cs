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
    internal class EgxEmployeeService : IEgxEmployeeService
    {
        private readonly IValidator<EgxEmployeeDto> _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EgxEmployeeService(IValidator<EgxEmployeeDto> validator,
            IUnitOfWork unitOfWork, IMapper mapper)
        {
            _validator = validator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateEgxEmployeeAsync(EgxEmployeeDto command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var existingEmp = await _unitOfWork.EgxEmployeeRepository.GetAsyncById(command.EmpCode);
            if (existingEmp != null)
            {
                throw new BadRequestException($"اسم الموظف '{command.EmpName}' موجود بالفعل.");
            }

            // 3. التحويل والحفظ
            var employee = _mapper.Map<EmpEgx>(command);

            await _unitOfWork.EgxEmployeeRepository.AddAsync(employee);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات الموظف.");
            }

            return employee.EmpCode; // إرجاع الكود الناتج
        }

        public async Task DeleteEgxEmployeeAsync(int id)
        {
            var employee = await _unitOfWork.EgxEmployeeRepository.GetAsyncById(id);
            if (employee == null)
            {
                throw new NotFoundException($"الموظف غير موجود.");
            }

            await _unitOfWork.EgxEmployeeRepository.DeleteAsync(employee);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<EgxEmployeeDto>> GetAllEgxEmployeeAsync()
        {
            var employees = await _unitOfWork.EgxEmployeeRepository.GetAllAsync();
            return _mapper.Map<IReadOnlyList<EgxEmployeeDto>>(employees);
        }

        public async Task<EgxEmployeeDto> GetEgxEmployeeByIdAsync(int id)
        {
            var employee = await _unitOfWork.EgxEmployeeRepository.GetAsyncById(id);
            if (employee == null) throw new NotFoundException("الموظف غير موجود");

            return _mapper.Map<EgxEmployeeDto>(employee);
        }

        // إضافة ميزة البحث والترقيم (Pagination) بنفس منطق الأصناف
        //public async Task<PagedResult<EgxEmployeeDto>> GetEmployeesPaginated(int page, string search)
        //{
        //    int pageSize = 20;
        //    string cleanSearch = search?.Trim() ?? "";

        //    // بناء الفلتر للبحث بالاسم أو الكود
        //    Expression<Func<EgxEmployee, bool>> filter = x =>
        //        (string.IsNullOrEmpty(cleanSearch) ||
        //         x.EmpName.Contains(cleanSearch) ||
        //         x.EmpCode.ToString().Contains(cleanSearch));

        //    var pagedResult = await _unitOfWork.EgxEmployeeRepository.GetPagedAsync(
        //        page,
        //        pageSize,
        //        filter: filter,
        //        orderBy: x => x.EmpName
        //    );

        //    if (pagedResult == null || pagedResult.Items == null)
        //    {
        //        return new PagedResult<EgxEmployeeDto> { Items = new List<EgxEmployeeDto>(), TotalCount = 0 };
        //    }

        //    return new PagedResult<EgxEmployeeDto>
        //    {
        //        Items = _mapper.Map<IReadOnlyList<EgxEmployeeDto>>(pagedResult.Items),
        //        TotalCount = pagedResult.TotalCount,
        //        PageNumber = pagedResult.PageNumber,
        //        PageSize = pagedResult.PageSize
        //    };
        //}

        public async Task UpdateEgxEmployeeAsync(EgxEmployeeDto command)
        {
            // 1. التحقق من البيانات
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // 2. التأكد من وجود الموظف
            var existingEmployee = await _unitOfWork.EgxEmployeeRepository.GetAsyncById(command.EmpCode);
            if (existingEmployee == null)
            {
                throw new NotFoundException($"الموظف ذو الكود '{command.EmpCode}' غير موجود.");
            }

            // 3. التحديث باستخدام AutoMapper
            _mapper.Map(command, existingEmployee);
            await _unitOfWork.EgxEmployeeRepository.UpdateAsync(existingEmployee);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
