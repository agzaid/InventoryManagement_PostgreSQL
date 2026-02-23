using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
//using Oracle.ManagedDataAccess.Client;
using System.Linq.Expressions;

namespace Persistance.Repositories
{
    internal class MonthlyConsumRepository : GenericRepository<MonthlyConsum>, IMonthlyConsumRepository
    {
        private readonly InventoryManagementDbContext _dbContext;
        public MonthlyConsumRepository(InventoryManagementDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
