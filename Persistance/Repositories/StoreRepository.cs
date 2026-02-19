using Application.Interfaces.Contracts.Persistance;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Persistance.Repositories
{
    internal class
        StoreRepository : GenericRepository<Store>, IStoreRepository
    {
        private readonly InventoryManagementDbContext _dbContext;

        public StoreRepository(InventoryManagementDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
