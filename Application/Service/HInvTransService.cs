using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using Application.Interfaces.Models.Pagination;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentValidation;
using System.Linq.Expressions;

namespace Application.Service
{
    internal class HInvTransService : IHInvTransService
    {
        private readonly IValidator<HInvTransDto> _validator;
        private readonly IValidator<InwardCreationDto> _inwardValidator;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public HInvTransService(IValidator<HInvTransDto> validator, IMapper mapper,
            IUnitOfWork unitOfWork, IValidator<InwardCreationDto> inwardValidator)
        {
            _validator = validator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _inwardValidator = inwardValidator;
        }

   public async Task<PagedResult<TransactionDisplayDto>> GetHistoryTransactionsPaginatedAsync(
     List<int> trTypes,
     int page,
     int pageSize,
     DateTime start,
     DateTime end)
        {
         Expression<Func<HInvTrans, bool>> filter = it =>
                trTypes.Contains(it.TrType) &&
                it.StoreCode == 1 &&
                it.TrDate2 >= start &&
                it.TrDate2 <= end;

            var pagedResult = await _unitOfWork.HInvTransRepository.GetPagedAsync(
                page,
                pageSize,
                filter: filter,
                orderBy: it => it.TrNum,
                descending: true,
                includeProperties: "Item,Supplier"
            );

            if (pagedResult == null || pagedResult.Items == null)
            {
                return new PagedResult<TransactionDisplayDto>
                {
                    Items = new List<TransactionDisplayDto>(),
                    TotalCount = 0,
                    PageNumber = page,
                    PageSize = pageSize
                };
            }

            var mappedItems = pagedResult.Items.Select(it => new TransactionDisplayDto
            {
                TrDate2 = it.TrDate2?.ToString("yyyy-MM-dd"),
                TrNum = it.TrNum,
                TrSerial = it.TrSerial,
                SupplierCode = it.SuplierCode,
                // تأكد من أن الخاصية في موديل Supplier هي SuplierDesc كما في الكود الخاص بك
                SupplierDesc = it.Supplier?.SuplierDesc ?? "---",
                ItemCode = it.ItemCode,
                ItemDesc = it.Item?.ItemDesc ?? "---",
                ItemQnt = it.ItemQnt,
                ItemPrice = it.ItemPrice,
                BillNum = it.BillNum?.ToString(),
                OrderDate = it.OrderDate,
                DeliverDate = it.DeliverDate,
                DeliverNo = it.DeliverNo?.ToString()
            }).ToList();

            return new PagedResult<TransactionDisplayDto>
            {
                Items = mappedItems,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<List<TransactionDisplayDto>> GetHistoryTransactionsEmployeeAsync(int storeCode)
        {
            var data = await _unitOfWork.HInvTransRepository.GetAllAsyncExpression(
                filter: it => it.TrType == 2 && it.StoreCode == storeCode,
                orderBy: it => it.TrNum
            );

            return data.Select(it => new TransactionDisplayDto
            {
                TrDate2 = it.TrDate2?.ToString("yyyy-MM-dd"),
                TrNum = it.TrNum,
                TrSerial = it.TrSerial,
                SupplierCode = it.SuplierCode,
                ItemCode = it.ItemCode,
                ItemQnt = it.ItemQnt,
                ItemPrice = it.ItemPrice,
                BillNum = it.BillNum.ToString(),
                OrderDate = it.OrderDate,
                DeliverDate = it.DeliverDate,
                DeliverNo = it.DeliverNo.ToString()
            }).ToList();
        }

        public async Task<int> CreateHInvTransAsync(InwardCreationDto command)
        {
            var validationResult = await _inwardValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (command.Items == null || !command.Items.Any(x => x.ItemQnt > 0))
            {
                throw new BadRequestException("يجب إضافة صنف واحد على الأقل بكمية أكبر من الصفر.");
            }

            var transactionsToSave = new List<HInvTrans>();
            int serialCounter = 1;
            int storeCode = 1;

            foreach (var itemDto in command.Items.Where(x => x.ItemQnt > 0))
            {
                var hInvTrans = new HInvTrans
                {
                    StoreCode = storeCode,
                    TrType = command.TrType,
                    TrDate = DateTime.Now.Date,
                    TrSerial = serialCounter,
                    ItemCode = itemDto.ItemCode,
                    SuplierCode = command.SuplierCode,
                    ItemQnt = itemDto.ItemQnt,
                    ItemPrice = itemDto.ItemPrice,
                    BillNum = int.TryParse(command.BillNum, out int bNum) ? bNum : 0,
                    DeliverDate = null,
                    TrDate2 = DateTime.Now.Date,
                    DepCode = command.DeptCode,
                    EmpCode = command.EmpCode,
                    FromToStore = 0,
                    TrNum2 = 0,
                    DeliverNo = 0
                };

                transactionsToSave.Add(hInvTrans);
                serialCounter++;
            }

            try
            {
                await _unitOfWork.HInvTransRepository.AddRangeAsync(transactionsToSave);
                var result = await _unitOfWork.SaveChangesAsync();

                if (result <= 0)
                {
                    throw new BadRequestException("لم يتم حفظ البيانات في قاعدة البيانات.");
                }

                return transactionsToSave.First().TrNum;
            }
            catch (Exception ex)
            {
                throw new BadRequestException("حدث خطأ تقني أثناء حفظ المستند: " + ex.Message);
            }
        }

        public async Task DeleteHInvTransAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Invalid ID provided for deletion.");
            }

            var hInvTrans = await _unitOfWork.HInvTransRepository.GetByTrNumAsync(id);

            if (hInvTrans == null)
            {
                throw new NotFoundException($"Transaction with ID '{id}' was not found.");
            }

            await _unitOfWork.HInvTransRepository.DeleteAsync(hInvTrans);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception("حدث خطأ أثناء محاولة حذف البيانات من قاعدة البيانات.");
            }
        }

        public async Task<IReadOnlyList<HInvTransDto>> GetAllHInvTransAsync()
        {
            var hInvTrans = await _unitOfWork.HInvTransRepository.GetAllAsync();
            return _mapper.Map<IReadOnlyList<HInvTransDto>>(hInvTrans);
        }

        public async Task<HInvTransDto> GetHInvTransByIdAsync(int id)
        {
            var getHInvTrans = await _unitOfWork.HInvTransRepository.GetAsyncById(id);
            if (getHInvTrans == null)
                throw new NotFoundException($"Transaction with code '{id}' was not found.");

            var modifiedHInvTrans = _mapper.Map<HInvTransDto>(getHInvTrans);
            return modifiedHInvTrans;
        }

        public async Task UpdateHInvTransAsync(HInvTransDto command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var existingTrans = await _unitOfWork.HInvTransRepository.GetAsyncById(command.TrNum);

            if (existingTrans == null)
            {
                throw new BadRequestException($"السجل رقم '{command.TrNum}' غير موجود للتعديل.");
            }

            _mapper.Map(command, existingTrans);

            await _unitOfWork.HInvTransRepository.UpdateAsync(existingTrans);

            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("لم يتم إجراء أي تغييرات على البيانات.");
            }
        }
        public async Task<List<TransactionDisplayDto>> GetHistoryByTypeAsync(int storeCode, TrType type)
        {
            var data = await _unitOfWork.HInvTransRepository.GetAllAsyncExpression(
                filter: it => it.StoreCode == storeCode && it.TrType == (int)type,
                orderBy: it => it.TrNum
            );

            // Use AutoMapper or manual mapping
            return data.Select(it => new TransactionDisplayDto
            {
                TrNum = it.TrNum,
                TrDate2 = it.TrDate2?.ToString("yyyy-MM-dd"),
                ItemCode = it.ItemCode,
                ItemQnt = it.ItemQnt,
                ItemPrice = it.ItemPrice,
                SupplierCode = it.SuplierCode,
                // If you need SupplierDesc or ItemDesc, you'll need to Join tables in the Repository
            }).ToList();
        }
        public async Task<List<TransactionDisplayDto>> GetHistoryTransactionsByTypeAndDateAsync(int storeCode, int trType, DateTime from, DateTime to)
        {
            // استدعاء الدالة الحالية دون تغييرها
            var data = await _unitOfWork.HInvTransRepository.GetAllAsyncExpression(
                filter: it => it.StoreCode == storeCode && it.TrType == trType && it.TrDate2 >= from && it.TrDate2 <= to,
                orderBy: it => it.TrNum,
                descending: true,
                includeProperties: "Item,Supplier,Department", // هنا نمرر الجداول المرتبطة
                tracked: false
            );

            // تحويل البيانات إلى DTO
            return data.Select(it => new TransactionDisplayDto
            {
                TrNum = it.TrNum,
                ItemCode = it.ItemCode,
                ItemDesc = it.Item?.ItemDesc ?? "غير معرف", // الاسم من جدول الأصناف
                SupplierCode = it.SuplierCode,
                SupplierDesc = it.Supplier?.SuplierDesc ?? "غير معرف", // الاسم من جدول الموردين
                ItemQnt = it.ItemQnt,
                ItemPrice = it.ItemPrice,
                TrDate2 = it.TrDate2?.ToString("yyyy-MM-dd"),
                //DepDesc = it.Department?.DepDesc ?? "" // اسم الإدارة أو المخزن المحول إليه
            }).ToList();
        }
    }
}
