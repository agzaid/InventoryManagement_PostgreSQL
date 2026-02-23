using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
//using Oracle.ManagedDataAccess.Client;

namespace Persistance.Repositories
{
    internal class OpenBalanceRepository : GenericRepository<OpenBalance>, IOpenBalanceRepository
    {
        private readonly InventoryManagementDbContext _dbContext;
        public OpenBalanceRepository(InventoryManagementDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        //public async Task<ItemCategory?> GetByNameAsync(string name)
        //{
        //    var itemCategory = await _dbContext.ItemCategories
        //                   .FirstOrDefaultAsync(d => d.CatgryDesc == name);
        //    return itemCategory;
        //}
        //public async Task<ItemCategory?> GetByIdStringAsync(string name)
        //{
        //    var itemCategory = await _dbContext.ItemCategories
        //                   .FirstOrDefaultAsync(d => d.CatgryCode == name);
        //    return itemCategory;
        //}
    }
}
