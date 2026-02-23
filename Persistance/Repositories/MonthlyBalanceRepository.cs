using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
//using Oracle.ManagedDataAccess.Client;

namespace Persistance.Repositories
{
    internal class MonthlyBalanceRepository : GenericRepository<MonthlyBalance>, IMonthlyBalanceRepository
    {
        private readonly InventoryManagementDbContext _dbContext;
        public MonthlyBalanceRepository(InventoryManagementDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
