using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Linq.Expressions;

namespace Persistance.Repositories
{
    internal class HInvTransRepository : GenericRepository<HInvTrans>, IHInvTransRepository
    {
        private readonly InventoryManagementDbContext _dbContext;
        public HInvTransRepository(InventoryManagementDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteRangeAsync(Expression<Func<HInvTrans, bool>> predicate)
        {
            var records = await _dbContext.HInvTrans.Where(predicate).ToListAsync();

            if (records.Any())
            {
                _dbContext.HInvTrans.RemoveRange(records);
            }
        }

        public async Task<HInvTrans?> GetByTrNumAsync(int id)
        {
            var item = await _dbContext.HInvTrans
                           .FirstOrDefaultAsync(d => d.TrNum == id);
            return item;
        }
    }
}
