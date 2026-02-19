using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Persistance.Repositories
{
    internal class ItemBalanceRepository : GenericRepository<ItemBalance>, IItemBalanceRepository
    {
        private readonly InventoryManagementDbContext _dbContext;
        public ItemBalanceRepository(InventoryManagementDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
