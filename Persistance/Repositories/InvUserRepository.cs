using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
//using Oracle.ManagedDataAccess.Client;

namespace Persistance.Repositories
{
    internal class InvUserRepository : GenericRepository<InvUser>, IInvUserRepository
    {
        private readonly InventoryManagementDbContext _dbContext;
        public InvUserRepository(InventoryManagementDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<InvUser>> GetAllUsersWithEmployeeDetailsAsync()
        {
            var users = await GetAllAsyncExpression<int>(
                filter: null,
                orderBy: u => u.UserCode,
                descending: false,
                includeProperties: "Employee",
                tracked: false
            );
            return users;
        }

        public async Task<InvUser> GetInvUserByCodeAsync(int code)
        {
            var users = await GetAllAsyncExpression<int>(
                filter: u => u.UserCode == code,
                orderBy: null,
                descending: false,
                includeProperties: "Employee",
                tracked: false
            );

            // Return the first match, or null if not found
            return users.FirstOrDefault();
        }
    }
}
