using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface IUnitOfWork : IDisposable
    {
        IDepartmentRepository DepartmentRepository { get; }
        IInvUserRepository InvUserRepository{ get; }
        IEgxEmployeeRepository EgxEmployeeRepository{ get; }
        IStoreRepository StoreRepository{ get; }
        IInvTransRepository InvTransRepository{ get; }
        IItemBalanceRepository ItemBalanceRepository{ get; }
        IMonthlyConsumRepository MonthlyConsumRepository { get; }
        IMonthlyBalanceRepository MonthlyBalanceRepository { get; }
        IHInvTransRepository HInvTransRepository{ get; }
        IItemCategoryRepository ItemCategoryRepository { get; }
        IItemRepository ItemRepository { get; }
        IOpenBalanceRepository OpenBalanceRepository { get; }
        ISupplierRepository SupplierRepository { get; }
        Task<int> SaveChangesAsync();
        Task<ITransactionWrapper> BeginTransactionAsync();
    }
}
