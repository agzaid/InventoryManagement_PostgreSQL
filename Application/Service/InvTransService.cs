using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using FluentValidation;

namespace Application.Service
{
    internal class InvTransService : IInvTransService
    {
        private readonly IValidator<InvTransDto> _validator;
        private readonly IValidator<InwardCreationDto> _inwardValidator;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public InvTransService(IValidator<InvTransDto> validator, IMapper mapper,
            IUnitOfWork unitOfWork, IValidator<InwardCreationDto> inwardValidator)
        {
            _validator = validator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _inwardValidator = inwardValidator;
        }
        public async Task<List<TransactionDisplayDto>> GetInventoryTransactionsAsync(int storeCode)
        {
            var data = await _unitOfWork.InvTransRepository.GetAllAsyncExpression(
                filter: it => it.TrType == 1 && it.StoreCode == storeCode,
                orderBy: it => it.TrNum,
                includeProperties: "Item,Supplier"
            );

            // التحويل إلى DTO
            return data.Select(it => new TransactionDisplayDto
            {
                TrDate2 = it.TrDate2?.ToString("yyyy-MM-dd"),
                TrNum = it.TrNum,
                TrSerial = it.TrSerial,
                SupplierCode = it.SuplierCode,
                SupplierDesc = it.Supplier?.SuplierDesc,
                ItemCode = it.ItemCode,
                ItemDesc = it.Item?.ItemDesc,
                ItemQnt = it.ItemQnt,
                ItemPrice = it.ItemPrice,
                BillNum = it.BillNum.ToString(),
                OrderDate = it.OrderDate,
                DeliverDate = it.DeliverDate,
                DeliverNo = it.DeliverNo.ToString()
            }).ToList();
        }
        public async Task<List<TransactionDisplayDto>> GetInventoryTransactionsEmployeeAsync(int storeCode)
        {
            var data = await _unitOfWork.InvTransRepository.GetAllAsyncExpression(
                     filter: it => it.TrType == 2 && it.StoreCode == storeCode,//&& it.TrDate.Date == DateTime.Today,
                     orderBy: it => it.TrNum,
                     includeProperties: "Item,Department,Employee"
                 );

            return data.Select(it => new TransactionDisplayDto
            {
                TrDate2 = it.TrDate2?.ToString("yyyy-MM-dd"),
                TrNum = it.TrNum,
                TrSerial = it.TrSerial,
                SupplierCode = it.SuplierCode,
                SupplierDesc = it.Supplier?.SuplierDesc,
                ItemCode = it.ItemCode,
                ItemDesc = it.Item?.ItemDesc,
                ItemQnt = it.ItemQnt,
                ItemPrice = it.ItemPrice,
                BillNum = it.BillNum.ToString(),
                OrderDate = it.OrderDate,
                DeliverDate = it.DeliverDate,
                DepDesc = it.Department?.DepDesc,
                EmpName = it.Employee?.EmpName,
                DeliverNo = it.DeliverNo.ToString()
            }).ToList();
        }

        public async Task<List<TransactionDisplayDto>> GetTransactionsByTypeAsync(int storeCode, int trType)
        {
            var data = await _unitOfWork.InvTransRepository.GetAllAsyncExpression(
                filter: it => it.TrType == trType && it.StoreCode == storeCode,
                orderBy: it => it.TrNum,
                includeProperties: "Item,Supplier,Department,Employee"
            );
            var distinctData = data.GroupBy(x => x.TrNum)
                       .Select(g => g.First())
                       .ToList();

            return distinctData.Select(it => new TransactionDisplayDto
            {
                TrDate2 = it.TrDate2?.ToString("yyyy-MM-dd"),
                TrNum = it.TrNum,
                TrSerial = it.TrSerial,
                SupplierCode = it.SuplierCode,
                SupplierDesc = it.Supplier?.SuplierDesc,
                ItemCode = it.ItemCode,
                ItemDesc = it.Item?.ItemDesc,
                ItemQnt = it.ItemQnt,
                ItemPrice = it.ItemPrice,
                BillNum = it.BillNum.ToString(),
                OrderDate = it.OrderDate,
                DeliverDate = it.DeliverDate,
                DepDesc = it.Department?.DepDesc,
                EmpName = it.Employee?.EmpName,
                DeliverNo = it.DeliverNo.ToString()
            }).ToList();
        }

        public async Task<IReadOnlyList<RecentActivityDto>> GetRecentActivityTodayAsync(int storeCode, int limit = 10)
        {
            var today = DateTime.Today;
            var data = await _unitOfWork.InvTransRepository.GetAllAsyncExpression(
                filter: it => it.StoreCode == storeCode && it.TrDate.Date == today,
                orderBy: it => it.TrNum,
                descending: true,
                includeProperties: "Item,Supplier,Department,Employee",
                tracked: false
            );

            var result = data
                .Take(limit)
                .Select(it => new RecentActivityDto
                {
                    TrNum = it.TrNum,
                    TrType = it.TrType,
                    TrTypeName = GetTrTypeName(it.TrType),
                    TrDate = it.TrDate2 ?? it.TrDate,
                    ItemCode = it.ItemCode,
                    ItemDesc = it.Item?.ItemDesc ?? it.ItemCode,
                    ItemQnt = it.ItemQnt,
                    Party = it.Supplier?.SuplierDesc
                            ?? it.Employee?.EmpName
                            ?? it.Department?.DepDesc
                            ?? string.Empty
                })
                .ToList();

            return result;
        }

        private static string GetTrTypeName(int trType)
        {
            return trType switch
            {
                1 => "Inward",
                2 => "Outward",
                3 => "Transfer Out",
                4 => "Transfer In",
                5 => "Return To Supplier",
                6 => "Return To Stock",
                7 => "Dead Stock",
                _ => "Unknown"
            };
        }

        public async Task<DailyInvTransSummaryDto> GetDailyInvTransSummaryTodayAsync(int storeCode)
        {
            var today = DateTime.Today;
            var data = await _unitOfWork.InvTransRepository.GetAllAsyncExpression(
                filter: it => it.StoreCode == storeCode && it.TrDate.Date == today,
                orderBy: it => it.TrNum,
                descending: true,
                tracked: false
            );

            return new DailyInvTransSummaryDto
            {
                Inward = data.Where(x => x.TrType == 1).Sum(x => x.ItemQnt),
                Outward = data.Where(x => x.TrType == 2).Sum(x => x.ItemQnt),
                TransferOut = data.Where(x => x.TrType == 3).Sum(x => x.ItemQnt),
                TransferIn = data.Where(x => x.TrType == 4).Sum(x => x.ItemQnt),
                ReturnToSupplier = data.Where(x => x.TrType == 5).Sum(x => x.ItemQnt),
                ReturnToStock = data.Where(x => x.TrType == 6).Sum(x => x.ItemQnt),
                DeadStock = data.Where(x => x.TrType == 7).Sum(x => x.ItemQnt)
            };
        }

        public async Task<int> CreateInvTransAsync(InwardCreationDto command)
        {
            // 1. Validation
            await ValidateCommand(command);

            int storeCode = 1;
            var transactionsToSave = new List<InvTrans>();

            foreach (var item in command.Items.Where(x => x.ItemQnt > 0))
            {
                // 2. Unified Balance Logic
                await UpdateItemBalance(item.ItemCode, storeCode, item.ItemQnt, command.TrType);

                // 3. Map Transaction Object
                transactionsToSave.Add(MapToInvTrans(command, item, storeCode));
            }

            // 4. Persistence
            try
            {
                await _unitOfWork.InvTransRepository.AddRangeAsync(transactionsToSave);
                var result = await _unitOfWork.SaveChangesAsync();

                if (result <= 0) throw new BadRequestException("لم يتم حفظ البيانات.");

                return transactionsToSave.First().TrNum;
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"حدث خطأ تقني: {ex.Message}");
            }
        }

        #region Helper Methods for create invTrans
        // --- Helper Methods to clean up the main flow ---

        private async Task ValidateCommand(InwardCreationDto command)
        {
            var validationResult = await _inwardValidator.ValidateAsync(command);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

            if (command.Items == null || !command.Items.Any(x => x.ItemQnt > 0))
            {
                throw new BadRequestException("يجب إضافة صنف واحد على الأقل بكمية أكبر من الصفر.");
            }
        }

        private async Task UpdateItemBalance(string itemCode, int storeCode, decimal qty, int trType)
        {
            var balanceRecords = await _unitOfWork.ItemBalanceRepository.GetAllAsyncExpression(
                filter: b => b.ItemCode == itemCode && b.StoreCode == storeCode,
                orderBy: b => b.BalDate,
                descending: true,
                tracked: true
            );

            var existingBalance = balanceRecords.FirstOrDefault();

            // Determine if this transaction adds to or subtracts from balance
            // 1 and 6 increase balance; 2, 3, 4, 5, 7 decrease it.
            decimal multiplier = (trType == 1 || trType == 6) ? 1 : -1;
            decimal currentQty = existingBalance?.CurrentBal ?? 0;

            // Check sufficiency for outgoing types
            if (multiplier == -1 && currentQty < qty)
            {
                throw new BadRequestException($"الرصيد غير كافٍ للصنف {itemCode}. الرصيد الحالي: {currentQty}");
            }

            bool shouldCreateNewRecord = (existingBalance == null || existingBalance.CurrentBal <= 0);

            if (shouldCreateNewRecord)
            {
                var newBalance = new ItemBalance
                {
                    StoreCode = storeCode,
                    ItemCode = itemCode,
                    OpenBal = currentQty,
                    CurrentBal = currentQty + (qty * multiplier),
                    BalDate = DateTime.Now
                };
                ApplyQuantityToType(newBalance, qty, trType);
                await _unitOfWork.ItemBalanceRepository.AddAsync(newBalance);
            }
            else
            {
                existingBalance.CurrentBal += (qty * multiplier);
                ApplyQuantityToType(existingBalance, qty, trType);
                await _unitOfWork.ItemBalanceRepository.UpdateAsync(existingBalance);
            }
        }

        private void ApplyQuantityToType(ItemBalance balance, decimal qty, int trType)
        {
            switch (trType)
            {
                case 1: balance.ItemIn += qty; break;
                case 2: balance.ItemOut += qty; break;
                case 3: balance.ItemTo += qty; break;
                case 4: balance.ItemBack2 += qty; break;
                case 5: balance.ItemBack += qty; break;
                case 6: balance.ItemBack2 += qty; break; // Note: You used ItemBack2 for both 4 and 6 in original code
                case 7: balance.ItemScrap += qty; break;
            }
        }

        private InvTrans MapToInvTrans(InwardCreationDto command, InwardItemDto itemDto, int storeCode)
        {
            return new InvTrans
            {
                StoreCode = storeCode,
                TrType = command.TrType,
                TrDate = DateTime.Now.Date,
                TrSerial = 1,
                ItemCode = itemDto.ItemCode,
                SuplierCode = command.SuplierCode,
                ItemQnt = itemDto.ItemQnt,
                ItemPrice = itemDto.ItemPrice,
                BillNum = int.TryParse(command.BillNum, out int bNum) ? bNum : 0,
                TrDate2 = DateTime.Now.Date,
                DepCode = command.DeptCode,
                EmpCode = command.EmpCode,
            };
        }
        #endregion
        public async Task DeleteInvTransAsync(int id)
        {
            // 1. Validate Input
            if (id <= 0)
            {
                throw new BadRequestException("Invalid ID provided for deletion.");
            }
            var invTrans = await _unitOfWork.InvTransRepository.GetByTrNumAsync(id);

            if (invTrans == null)
            {
                throw new NotFoundException($"Transaction with ID '{id}' was not found.");
            }

            // 2. Restore ItemBalance based on TrType
            var balanceRecords = await _unitOfWork.ItemBalanceRepository.GetAllAsyncExpression(
                filter: b => b.ItemCode == invTrans.ItemCode && b.StoreCode == invTrans.StoreCode,
                orderBy: b => b.BalDate,
                descending: true
            );
            var currentBalance = balanceRecords.FirstOrDefault();

            if (currentBalance != null)
            {
                decimal newCurrentBal = currentBalance.CurrentBal;
                decimal itemIn = 0;
                decimal itemOut = 0;
                decimal itemFrom = 0;
                decimal itemTo = 0;

                // TrType = 1 (Inward/Entry): ItemIn was added, so subtract it back
                if (invTrans.TrType == 1)
                {
                    newCurrentBal = currentBalance.CurrentBal - invTrans.ItemQnt;
                    itemIn = -invTrans.ItemQnt; // Negative to reverse
                }
                // TrType = 2 (Outgoing): ItemOut was subtracted, so add it back
                else if (invTrans.TrType == 2)
                {
                    newCurrentBal = currentBalance.CurrentBal + invTrans.ItemQnt;
                    itemOut = -invTrans.ItemQnt; // Negative to reverse
                }
                // TrType = 3 (Transfer Out): ItemTo was used, add it back
                else if (invTrans.TrType == 3)
                {
                    newCurrentBal = currentBalance.CurrentBal + invTrans.ItemQnt;
                    itemTo = -invTrans.ItemQnt; // Negative to reverse
                }
                // TrType = 4 (Transfer In / Returned): ItemFrom was used, subtract it
                else if (invTrans.TrType == 4)
                {
                    newCurrentBal = currentBalance.CurrentBal - invTrans.ItemQnt;
                    itemFrom = -invTrans.ItemQnt; // Negative to reverse
                }
                // TrType = 5 (Incoming Items Return): Items returned to supplier, so add them back
                else if (invTrans.TrType == 5)
                {
                    newCurrentBal = currentBalance.CurrentBal + invTrans.ItemQnt;
                    itemOut = -invTrans.ItemQnt; // Negative to reverse
                }
                // TrType = 6 (Issued Items Return): Items returned to stock, so subtract them back
                else if (invTrans.TrType == 6)
                {
                    newCurrentBal = currentBalance.CurrentBal - invTrans.ItemQnt;
                    itemIn = -invTrans.ItemQnt; // Negative to reverse
                }
                // TrType = 7 (Dead Stock): Items removed as dead stock, so add them back
                else if (invTrans.TrType == 7)
                {
                    newCurrentBal = currentBalance.CurrentBal + invTrans.ItemQnt;
                    itemOut = -invTrans.ItemQnt; // Negative to reverse
                }

                // Add new balance record with reversed values
                await _unitOfWork.ItemBalanceRepository.AddAsync(new ItemBalance
                {
                    StoreCode = invTrans.StoreCode,
                    ItemCode = invTrans.ItemCode,
                    OpenBal = currentBalance.OpenBal,
                    CurrentBal = newCurrentBal,
                    ItemIn = itemIn,
                    ItemOut = itemOut,
                    ItemFrom = itemFrom,
                    ItemTo = itemTo,
                    BalDate = DateTime.Now
                });
            }

            // 3. Delete the transaction
            await _unitOfWork.InvTransRepository.DeleteAsync(invTrans);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception("حدث خطأ أثناء محاولة حذف البيانات من قاعدة البيانات.");
            }
        }

        public async Task<IReadOnlyList<InvTransDto>> GetAllInvTransAsync()
        {
            var invTrans = await _unitOfWork.InvTransRepository.GetAllAsync();

            return _mapper.Map<IReadOnlyList<InvTransDto>>(invTrans);
        }

        public async Task<InvTransDto> GetInvTransByIdAsync(int id)
        {
            var getInvTrans = await _unitOfWork.InvTransRepository.GetAllAsyncExpression(filter: s=>s.TrNum == id,orderBy:s=>s.TrDate);
            var invtrans = getInvTrans.FirstOrDefault();
            if (getInvTrans == null)
                throw new NotFoundException($"Supplier with code '{id}' was not found.");

            var modifiedInvTrans = _mapper.Map<InvTransDto>(invtrans);
            return modifiedInvTrans;
        }


        public async Task UpdateInvTransAsync(InvTransDto command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            var existingTrans = await _unitOfWork.InvTransRepository.GetAsyncById(command.TrNum);

            if (existingTrans == null)
            {
                throw new BadRequestException($"السجل رقم '{command.TrNum}' غير موجود للتعديل.");
            }
            _mapper.Map(command, existingTrans);

            // Safety: Prevent EF from trying to update or insert related Master Data
            // We only want to update the Foreign Key IDs, not the actual objects
            existingTrans.Item = null;
            existingTrans.Supplier = null;
            existingTrans.Employee = null;
            existingTrans.Department = null;

            await _unitOfWork.InvTransRepository.UpdateAsync(existingTrans);

            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("لم يتم إجراء أي تغييرات على البيانات.");
            }
        }
    }
}
