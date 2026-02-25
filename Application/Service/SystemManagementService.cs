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
    internal class SystemManagementService : ISystemManagementService
    {
        private readonly IInvUserRepository _invUserRepository;
        private readonly IEgxEmployeeRepository _egxEmployeeRepository;
        private readonly IValidator<EgxEmployeeDto> _validator;
        private readonly IMapper _mapper;

        private readonly IUnitOfWork _unitOfWork;

        public SystemManagementService(IInvUserRepository invUserRepository, IEgxEmployeeRepository employeeRepository, IMapper mapper, IValidator<EgxEmployeeDto> validator,
            IUnitOfWork unitOfWork)
        {
            _invUserRepository = invUserRepository;
            _egxEmployeeRepository = employeeRepository;
            _validator = validator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateInvUserAsync(InvUserDto command)
        {
            command.UserCode = Convert.ToInt32(command.FullNameArabic);

            var employees = await _unitOfWork.EgxEmployeeRepository.GetAllAsyncExpression(
                                             filter: s => s.EmpCode == command.UserCode,
                                             orderBy: s => s.EmpCode,
                                             tracked: false);
            var employee = employees.FirstOrDefault();
            if (employee == null)
            {
                throw new BadRequestException("Employee code does not exist in the human resources system.");
            }
            var newUser = _mapper.Map<InvUser>(command);
            await _unitOfWork.InvUserRepository.AddAsync(newUser);
            int rowsAffected = await _unitOfWork.SaveChangesAsync();

            return rowsAffected;
        }

        public async Task<int> UpdateInvUserAsync(InvUserDto command)
        {
            var invUser = await _unitOfWork.InvUserRepository.GetInvUserByCodeAsync((int)command.UserCode);
            if (invUser == null)
            {
                throw new BadRequestException("invUser code does not exist in the human resources system.");
            }
            command.Id = invUser.Id;
            var updateUser = _mapper.Map(command, invUser);
            await _unitOfWork.InvUserRepository.UpdateAsync(updateUser);
            int rowsAffected = await _unitOfWork.SaveChangesAsync();

            return rowsAffected;
        }
        public async Task<int> DeleteInvUserAsync(int usercode)
        {
            var invUser = await _unitOfWork.InvUserRepository.GetInvUserByCodeAsync(usercode);
            if (invUser == null)
            {
                throw new BadRequestException("UserNotFound");
            }
            await _unitOfWork.InvUserRepository.DeleteAsync(invUser);
            int rowsAffected = await _unitOfWork.SaveChangesAsync();

            return rowsAffected;
        }
        public Task DeleteEgxEmployeeAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<SysMangementIndexDto> GetAllEgxEmployeesAsync()
        {
            //var users = await _invUserRepository.GetAllUsersWithEmployeeDetailsAsync();
            var users = await _unitOfWork.InvUserRepository.GetAllUsersWithEmployeeDetailsAsync();
            var mappedUsers = _mapper.Map<IReadOnlyList<InvUserDto>>(users);
            var egxEmployee = await _egxEmployeeRepository.GetAllAsyncExpression(null, s => s.EmpName);
            var mappedEgxEmployee = _mapper.Map<IReadOnlyList<EgxEmployeeDto>>(egxEmployee);
            var sysmanagement = new SysMangementIndexDto()
            {
                invUserDto = mappedUsers,
                EgxEmployeeDto = mappedEgxEmployee
            };
            return sysmanagement;
        }

        public async Task<(bool Success, string Message)> RelayDataAsync(int storeCode, DateTime systemDate)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            // 1. جلب البيانات مرة واحدة فقط
            var transactionsToRelay = await _unitOfWork.InvTransRepository.GetAllAsyncExpression(
                filter: s => s.StoreCode == storeCode,
                orderBy: a => a.StoreCode);

            if (!transactionsToRelay.Any())
                return (false, "لا توجد بيانات متاحة للترحيل لهذا المستودع.");

            // 2. غلق النظام (Lock)
            var stores = await _unitOfWork.StoreRepository.GetAllAsyncExpression(s => s.StoreCode == storeCode, a => a.StoreCode);
            var currentStore = stores.FirstOrDefault();
            if (currentStore != null) currentStore.SysLock = "0";

            // 3. أرشفة البيانات (Relay_Transactions)
            var historyRecords = transactionsToRelay.Select(t => _mapper.Map<HInvTrans>(t)).ToList();
            foreach (var h in historyRecords)
            {
                await _unitOfWork.HInvTransRepository.AddAsync(h);
            }

            // 4. تحديث أرقام الحركات القصوى (Relay_SetMax)
            await UpdateStoreMaxNumbersAsync(storeCode, transactionsToRelay, currentStore);

            // 5. معالجة الأرصدة اليومية (Relay_DailyBal + Relay_DailyBal2)
            await ProcessDailyBalancesAsync(storeCode, systemDate, transactionsToRelay);

            // 6. معالجة الأرصدة الشهرية (Relay_MonthlyBal + Relay_MonthlyBal2)
            await ProcessMonthlyBalancesAsync(storeCode, systemDate);

            // 7. معالجة الاستهلاك الشهري (Relay_MonthlyConsum)
            await ProcessConsumptionLinqAsync(storeCode, systemDate, historyRecords);

            // 8. حذف البيانات الأصلية (Relay_DelData)
            await _unitOfWork.InvTransRepository.DeleteRangeAsync(s => s.StoreCode == storeCode);

            // 9. فتح النظام وتحديث الحالة
            if (currentStore != null) currentStore.SysLock = "1";

            // حفظ الكل واعتماد المعاملة
            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, "تمت عملية الترحيل بنجاح.");
        }
        private async Task ProcessBalancesOptimizedAsync(int storeCode, DateTime date, IEnumerable<InvTrans> currentTrans)
        {
            var itemCodes = currentTrans.Select(x => x.ItemCode).Distinct().ToList();

            // Fetch existing balances for these items on this date in ONE call
            //var existingBalances = await _unitOfWork.Repository<ItemBalance>()
            //    .GetAllAsyncExpression(filter: b =>
            //        b.StoreCode == storeCode &&
            //        b.BalDate == date &&
            //        itemCodes.Contains(b.ItemCode));
            var existingBalance = await _unitOfWork.ItemBalanceRepository.GetAllAsyncExpression(filter: s => s.StoreCode == storeCode
                                                                                                            && s.BalDate == date
                                                                                                            && itemCodes.Contains(s.ItemCode),
                                                                                                            a => a.StoreCode);
            var existingItemCodes = existingBalance.Select(b => b.ItemCode).ToHashSet();

            foreach (var code in itemCodes)
            {
                if (!existingItemCodes.Contains(code))
                {
                    //await _unitOfWork.Repository<ItemBalance>().AddAsync(new ItemBalance
                    //{
                    //    StoreCode = storeCode,
                    //    ItemCode = code,
                    //    BalDate = date,
                    //    OpenBal = 0,
                    //    CurrentBal = 0
                    //});
                    await _unitOfWork.ItemBalanceRepository.AddAsync(new ItemBalance
                    {
                        StoreCode = storeCode,
                        ItemCode = code,
                        BalDate = date,
                        OpenBal = 0,
                        CurrentBal = 0,
                    });
                }
            }
        }

        private async Task ProcessConsumptionLinqAsync(int storeCode, DateTime date, IEnumerable<HInvTrans> history)
        {
            // تجميع البيانات حسب القسم والصنف (Group By)
            var groupedConsumption = history
                .Where(x => x.TrType == 2 && x.TrDate.Year == date.Year && x.TrDate.Month == date.Month)
                .GroupBy(x => new { x.DepCode, x.ItemCode })
                .Select(g => new
                {
                    g.Key.DepCode,
                    g.Key.ItemCode,
                    TotalQnt = g.Sum(x => x.ItemQnt),
                    AvgQnt = g.Average(x => x.ItemQnt)
                });

            foreach (var item in groupedConsumption)
            {
                //var consumRecord = (await _unitOfWork.Repository<MonthlyConsum>()
                //    .GetAllAsyncExpression(filter: c => c.StoreCode == storeCode && c.ItemCode == item.ItemCode
                //                            && c.DepCode == item.DepCode && c.ConsumYear == date.Year
                //                            && c.ConsumMonth == date.Month)).FirstOrDefault();
                var consumRecords = await _unitOfWork.MonthlyConsumRepository.GetAllAsyncExpression(filter: a => a.StoreCode == storeCode
                                                                                                                && a.ItemCode == item.ItemCode
                                                                                                                && a.DepCode == item.DepCode
                                                                                                                && a.ConsumYear == date.Year
                                                                                                                && a.ConsumMonth == date.Month, s => s.StoreCode);
                var consumRecord = consumRecords.FirstOrDefault();
                if (consumRecord != null)
                {
                    consumRecord.ConsumQnt = (int)item.TotalQnt;
                    consumRecord.ConsumAvg = (decimal)item.AvgQnt;
                }
                else
                {
                    await _unitOfWork.MonthlyConsumRepository.AddAsync(new MonthlyConsum()
                    {
                        StoreCode = storeCode,
                        ItemCode = item.ItemCode,
                        DepCode = (int)item.DepCode,
                        ConsumYear = date.Year,
                        ConsumMonth = date.Month,
                        ConsumQnt = (int)item.TotalQnt,
                        ConsumAvg = (decimal)item.AvgQnt
                    });
                    //await _unitOfWork.Repository<MonthlyConsum>().AddAsync(new MonthlyConsum
                    //{
                    //    StoreCode = storeCode,
                    //    ItemCode = item.ItemCode,
                    //    DepCode = item.DepCode,
                    //    ConsumYear = date.Year,
                    //    ConsumMonth = date.Month,
                    //    ConsumQnt = (int)item.TotalQnt,
                    //    ConsumAvg = (decimal)item.AvgQnt
                    //});
                }
            }
        }

        /// <summary>
        /// Relay_SetMax - تحديث أرقام الحركات القصوى في جدول المستودعات
        /// </summary>
        private async Task UpdateStoreMaxNumbersAsync(int storeCode, IEnumerable<InvTrans> transactions, Store currentStore)
        {
            if (currentStore == null) return;

            // Get max transaction numbers by type
            var maxByType = transactions
                .GroupBy(t => t.TrType)
                .ToDictionary(g => g.Key, g => g.Max(t => t.TrNum));

            // Update store with max numbers (TrType: 1=In, 2=Out, 3=To, 5=Back, 6=Back2, 7=Scrap)
            if (maxByType.TryGetValue(1, out var maxIn) && maxIn > 0) currentStore.InNum = maxIn;
            if (maxByType.TryGetValue(2, out var maxOut) && maxOut > 0) currentStore.OutNum = maxOut;
            if (maxByType.TryGetValue(3, out var maxTo) && maxTo > 0) currentStore.ToNum = maxTo;
            if (maxByType.TryGetValue(5, out var maxBack) && maxBack > 0) currentStore.BackNum = maxBack;
            if (maxByType.TryGetValue(6, out var maxBack2) && maxBack2 > 0) currentStore.BackNum2 = maxBack2;
            if (maxByType.TryGetValue(7, out var maxScrap) && maxScrap > 0) currentStore.ScrapNum = maxScrap;

            // Clear system date after relay
            currentStore.SysDate = null;
        }

        /// <summary>
        /// Relay_DailyBal + Relay_DailyBal2 - معالجة الأرصدة اليومية
        /// </summary>
        private async Task ProcessDailyBalancesAsync(int storeCode, DateTime systemDate, IEnumerable<InvTrans> transactions)
        {
            var itemCodes = transactions.Select(x => x.ItemCode).Distinct().ToList();

            foreach (var itemCode in itemCodes)
            {
                // Get the latest balance for this item (before today)
                var existingBalances = await _unitOfWork.ItemBalanceRepository.GetAllAsyncExpression(
                    filter: b => b.StoreCode == storeCode && b.ItemCode == itemCode,
                    orderBy: b => b.BalDate,
                    descending: true);

                var latestBalance = existingBalances.FirstOrDefault();
                decimal openingBalance = latestBalance?.CurrentBal ?? 0;

                // Check if balance record exists for today
                var todayBalance = existingBalances.FirstOrDefault(b => b.BalDate.Date == systemDate.Date);

                if (todayBalance == null)
                {
                    // Relay_DailyBal - Insert new balance record for today
                    todayBalance = new ItemBalance
                    {
                        StoreCode = storeCode,
                        ItemCode = itemCode,
                        BalDate = systemDate.Date,
                        OpenBal = openingBalance,
                        ItemIn = 0,
                        ItemOut = 0,
                        ItemFrom = 0,
                        ItemTo = 0,
                        ItemBack = 0,
                        ItemBack2 = 0,
                        ItemScrap = 0,
                        CurrentBal = openingBalance
                    };
                    await _unitOfWork.ItemBalanceRepository.AddAsync(todayBalance);
                }

                // Relay_DailyBal2 - Calculate sums by TrType for this item
                var itemTransactions = transactions.Where(t => t.ItemCode == itemCode).ToList();
                var sumByType = itemTransactions
                    .GroupBy(t => t.TrType)
                    .ToDictionary(g => g.Key, g => g.Sum(t => t.ItemQnt));

                decimal itemIn = sumByType.GetValueOrDefault(1, 0);      // TrType 1 = Inward
                decimal itemOut = sumByType.GetValueOrDefault(2, 0);     // TrType 2 = Outward
                decimal itemFrom = sumByType.GetValueOrDefault(3, 0);    // TrType 3 = Transfer Out
                decimal itemTo = sumByType.GetValueOrDefault(4, 0);      // TrType 4 = Transfer In
                decimal itemBack = sumByType.GetValueOrDefault(5, 0);    // TrType 5 = Incoming Return
                decimal itemBack2 = sumByType.GetValueOrDefault(6, 0);   // TrType 6 = Issued Return
                decimal itemScrap = sumByType.GetValueOrDefault(7, 0);   // TrType 7 = Dead Stock

                // Update balance record
                todayBalance.ItemIn = itemIn;
                todayBalance.ItemOut = itemOut;
                todayBalance.ItemFrom = itemFrom;
                todayBalance.ItemTo = itemTo;
                todayBalance.ItemBack = itemBack;
                todayBalance.ItemBack2 = itemBack2;
                todayBalance.ItemScrap = itemScrap;

                // Calculate current balance: OpenBal + (In + To + Back2) - (Out + From + Back + Scrap)
                todayBalance.CurrentBal = todayBalance.OpenBal + itemIn + itemTo + itemBack2 - itemOut - itemFrom - itemBack - itemScrap;
            }
        }

        /// <summary>
        /// Relay_MonthlyBal + Relay_MonthlyBal2 - معالجة الأرصدة الشهرية
        /// </summary>
        private async Task ProcessMonthlyBalancesAsync(int storeCode, DateTime systemDate)
        {
            int year = systemDate.Year;
            int month = systemDate.Month;

            // Get all item balances for this month
            var monthlyItemBalances = await _unitOfWork.ItemBalanceRepository.GetAllAsyncExpression(
                filter: b => b.StoreCode == storeCode && b.BalDate.Year == year && b.BalDate.Month == month,
                orderBy: b => b.ItemCode);

            // Group by item code and sum the values
            var itemSummaries = monthlyItemBalances
                .GroupBy(b => b.ItemCode)
                .Select(g => new
                {
                    ItemCode = g.Key,
                    // Get opening balance from the first record of the month
                    OpenBal = g.OrderBy(b => b.BalDate).First().OpenBal,
                    ItemIn = g.Sum(b => b.ItemIn),
                    ItemOut = g.Sum(b => b.ItemOut),
                    ItemFrom = g.Sum(b => b.ItemFrom),
                    ItemTo = g.Sum(b => b.ItemTo),
                    ItemBack = g.Sum(b => b.ItemBack),
                    ItemBack2 = g.Sum(b => b.ItemBack2),
                    ItemScrap = g.Sum(b => b.ItemScrap),
                    // Get current balance from the last record of the month
                    CurrentBal = g.OrderByDescending(b => b.BalDate).First().CurrentBal
                });

            foreach (var item in itemSummaries)
            {
                // Check if monthly balance record exists
                var existingMonthlyBalances = await _unitOfWork.MonthlyBalanceRepository.GetAllAsyncExpression(
                    filter: m => m.StoreCode == storeCode && m.BalYear == year && m.BalMonth == month && m.ItemCode == item.ItemCode,
                    orderBy: m => m.ItemCode);

                var monthlyBalance = existingMonthlyBalances.FirstOrDefault();

                if (monthlyBalance != null)
                {
                    // Update existing monthly balance
                    monthlyBalance.OpenBal = item.OpenBal;
                    monthlyBalance.ItemIn = item.ItemIn;
                    monthlyBalance.ItemOut = item.ItemOut;
                    monthlyBalance.ItemFrom = item.ItemFrom;
                    monthlyBalance.ItemTo = item.ItemTo;
                    monthlyBalance.ItemBack = item.ItemBack;
                    monthlyBalance.ItemBack2 = item.ItemBack2;
                    monthlyBalance.ItemScrap = item.ItemScrap;
                    monthlyBalance.CurrentBal = item.CurrentBal;
                }
                else
                {
                    // Insert new monthly balance
                    await _unitOfWork.MonthlyBalanceRepository.AddAsync(new MonthlyBalance
                    {
                        StoreCode = storeCode,
                        BalYear = year,
                        BalMonth = month,
                        ItemCode = item.ItemCode,
                        OpenBal = item.OpenBal,
                        ItemIn = item.ItemIn,
                        ItemOut = item.ItemOut,
                        ItemFrom = item.ItemFrom,
                        ItemTo = item.ItemTo,
                        ItemBack = item.ItemBack,
                        ItemBack2 = item.ItemBack2,
                        ItemScrap = item.ItemScrap,
                        CurrentBal = item.CurrentBal
                    });
                }
            }
        }
    }
}
