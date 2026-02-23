using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
//using Oracle.ManagedDataAccess.Client;

namespace Persistance.Repositories
{
    internal class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
    {
        private readonly InventoryManagementDbContext _dbContext;
        public SupplierRepository(InventoryManagementDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Supplier?> GetByNameAsync(string name)
        {
            var itemCategory = await _dbContext.Suppliers
                           .FirstOrDefaultAsync(d => d.SuplierDesc == name);
            return itemCategory;
        }
        public async Task<Supplier?> GetByIdStringAsync(string code)
        {
            var itemCategory = await _dbContext.Suppliers
                           .FirstOrDefaultAsync(d => d.SuplierCode == Convert.ToInt32(code));
            return itemCategory;
        }

      
    }
}
