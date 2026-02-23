using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
//using Oracle.ManagedDataAccess.Client;
using System.Linq.Expressions;

namespace Persistance.Repositories
{
    internal class InvTransRepository : GenericRepository<InvTrans>, IInvTransRepository
    {
        private readonly InventoryManagementDbContext _dbContext;
        public InvTransRepository(InventoryManagementDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteRangeAsync(Expression<Func<InvTrans, bool>> predicate)
        {
            // 1. Find the records in the database
            var records = await _dbContext.InvTrans.Where(predicate).ToListAsync();

            // 2. Mark them for deletion in the Change Tracker
            if (records.Any())
            {
                _dbContext.InvTrans.RemoveRange(records);
            }
        }
        public async Task<InvTrans?> GetByTrNumAsync(int id)
        {
            var itemCategory = await _dbContext.InvTrans
                           .FirstOrDefaultAsync(d => d.TrNum == id);
            return itemCategory;
        }
    }
}
