using Application.Interfaces.Constanst;
using Application.Interfaces.Contracts.Persistance;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InventoryManagementDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDepartmentRepository _departmentRepository;
        private IInvUserRepository _invUserRepository;
        private IInvTransRepository _invTransRepository;
        private IItemBalanceRepository _itemBalanceRepository;
        private IHInvTransRepository _hinvTransRepository;
        private IEgxEmployeeRepository _egxEmployeeRepository;
        private IStoreRepository _storeRepository;
        private IMonthlyConsumRepository _monthlyConsumRepository;
        private IMonthlyBalanceRepository _monthlyBalanceRepository;
        private IItemCategoryRepository _itemCategoryRepository;
        private IItemRepository _itemRepository;
        private IOpenBalanceRepository _openBalanceRepository;
        private ISupplierRepository _supplierRepository;

        public UnitOfWork(InventoryManagementDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public IDepartmentRepository DepartmentRepository => _departmentRepository ??= new DepartmentRepository(_context);

        public IInvUserRepository InvUserRepository => _invUserRepository ??= new InvUserRepository(_context);
        public IInvTransRepository InvTransRepository => _invTransRepository ??= new InvTransRepository(_context);
        public IItemBalanceRepository ItemBalanceRepository => _itemBalanceRepository ??= new ItemBalanceRepository(_context);
        public IHInvTransRepository HInvTransRepository => _hinvTransRepository ??= new HInvTransRepository(_context);

        public IEgxEmployeeRepository EgxEmployeeRepository => _egxEmployeeRepository ??= new EgxEmployeeRepository(_context);
        public IStoreRepository StoreRepository => _storeRepository ??= new StoreRepository(_context);

        public IMonthlyConsumRepository MonthlyConsumRepository => _monthlyConsumRepository ?? new MonthlyConsumRepository(_context);
        public IMonthlyBalanceRepository MonthlyBalanceRepository => _monthlyBalanceRepository ?? new MonthlyBalanceRepository(_context);
        public IItemCategoryRepository ItemCategoryRepository => _itemCategoryRepository ?? new ItemCategoryRepository(_context);
        public IItemRepository ItemRepository => _itemRepository ?? new ItemRepository(_context);
        public IOpenBalanceRepository OpenBalanceRepository => _openBalanceRepository ?? new OpenBalanceRepository(_context);
        public ISupplierRepository SupplierRepository => _supplierRepository ?? new SupplierRepository(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
        public async Task<ITransactionWrapper> BeginTransactionAsync()
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            return new EfTransactionWrapper(transaction);
        }
        public async Task<int> SaveChangesAsync()
        {
            var username = _httpContextAccessor.HttpContext.User.FindFirst(CustomClaimTypes.Uid)?.Value;
            return await _context.SaveChangesAsync(username);
        }
    }
}
