using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
//using Oracle.ManagedDataAccess.Client;

namespace Persistance.Repositories
{
    internal class EgxEmployeeRepository : GenericRepository<EmpEgx>, IEgxEmployeeRepository
    {
        private readonly InventoryManagementDbContext _dbContext;
        public EgxEmployeeRepository(InventoryManagementDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EmpEgx?> GetByIdStringAsync(string name)
        {
            var itemCategory = await _dbContext.EmpEgx
                           .FirstOrDefaultAsync(d => d.EmpName == name);
            return itemCategory;
        }

        //public async Task<IReadOnlyList<InvUser>> GetAllUsersWithEmployeeDetailsAsync()
        //{
        //    // We call your method and specify <object> or <int> for the sorting key
        //    // since you aren't actually using the orderBy parameter here.
        //    var users = await GetAllAsyncExpression<int>(
        //        filter: null,
        //        orderBy: u => u.UserCode, // We provide a default sort to satisfy TKey
        //        descending: false,
        //        includeProperties: "Employee", // This triggers the join you configured
        //        tracked: false
        //    );
        //    return users;
        //}
    }
}
