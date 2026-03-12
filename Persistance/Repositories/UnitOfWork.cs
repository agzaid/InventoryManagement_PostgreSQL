using Application.Interfaces.Constanst;
using Application.Interfaces.Contracts.Persistance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InventoryManagementDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
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
        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;
        private IRolePermissionRepository _rolePermissionRepository;
        private IUserRoleRepository _userRoleRepository;

        public UnitOfWork(InventoryManagementDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
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
        public IItemCategoryRepository ItemCategoryRepository => _itemCategoryRepository ??= new ItemCategoryRepository(_context);
        public IItemRepository ItemRepository => _itemRepository ??= new ItemRepository(_context);
        public IOpenBalanceRepository OpenBalanceRepository => _openBalanceRepository ??= new OpenBalanceRepository(_context);
        public ISupplierRepository SupplierRepository => _supplierRepository ??= new SupplierRepository(_context);
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context, _userManager);
        public IRoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_context, _roleManager);
        public IRolePermissionRepository RolePermissionRepository => _rolePermissionRepository ??= new RolePermissionRepository(_context);
        public IUserRoleRepository UserRoleRepository => _userRoleRepository ??= new UserRoleRepository(_context);

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
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
