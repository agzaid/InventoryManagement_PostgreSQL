using Application.Interface.Contract.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface IEgxEmployeeRepository : IGenericRepository<EmpEgx>
    {
        Task<EmpEgx?> GetByIdStringAsync(string code);
        //Task<IReadOnlyList<EmpEgx>> GetAllUsersWithEmployeeDetailsAsync();
    }
}
