using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
//using Oracle.ManagedDataAccess.Client;

namespace Persistance.Repositories
{
    internal class ItemRepository : GenericRepository<Item>, IItemRepository
    {
        private readonly InventoryManagementDbContext _dbContext;
        public ItemRepository(InventoryManagementDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Item?> GetByNameAsync(string name)
        {
            var itemCategory = await _dbContext.Items
                           .FirstOrDefaultAsync(d => d.ItemDesc == name);
            return itemCategory;
        }
        public async Task<Item?> GetByIdStringAsync(string name)
        {
            var itemCategory = await _dbContext.Items
                           .FirstOrDefaultAsync(d => d.ItemCode == name);
            return itemCategory;
        }

        public async Task<IReadOnlyList<Item>> GetAllItemsWithCategory()
        {
            var items = await GetAllAsyncExpression<string>(filter: null, orderBy: u => u.ItemCode, descending: true, includeProperties: "ItemCategory", tracked: false);
            return items;
           // var users = await GetAllAsyncExpression<int>(
           //    filter: null,
           //    orderBy: u => u.UserCode, // We provide a default sort to satisfy TKey
           //    descending: false,
           //    includeProperties: "Employee", // This triggers the join you configured
           //    tracked: false
           //);
           //return users;
        }
    }
}
