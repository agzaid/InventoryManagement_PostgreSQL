using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
//using Oracle.ManagedDataAccess.Client;

namespace Persistance.Repositories
{
    internal class ItemCategoryRepository : GenericRepository<ItemCategory>, IItemCategoryRepository
    {
        private readonly InventoryManagementDbContext _dbContext;
        public ItemCategoryRepository(InventoryManagementDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ItemCategory?> GetByNameAsync(string name)
        {
            var itemCategory = await _dbContext.ItemCategories
                           .FirstOrDefaultAsync(d => d.CatgryDesc == name);
            return itemCategory;
        }
        public async Task<ItemCategory?> GetByIdStringAsync(string name)
        {
            if (int.TryParse(name, out int catgryCode))
            {
                var itemCategory = await _dbContext.ItemCategories
                               .FirstOrDefaultAsync(d => d.CatgryCode == catgryCode);
                return itemCategory;
            }
            return null;
        }
    }
}
