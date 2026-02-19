using Application.Interfaces.Contracts.Persistance;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Persistance.Repositories
{
    internal class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        private readonly InventoryManagementDbContext _dbContext;
        public DepartmentRepository(InventoryManagementDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Department?> GetByNameAsync(string name)
        {
            var department = await _dbContext.Departments
                           .FirstOrDefaultAsync(d => d.DepDesc == name);
            return department;
        }
    }
}
