using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface IInvUserRepository : IGenericRepository<InvUser>
    {
        Task<IReadOnlyList<InvUser>> GetAllUsersWithEmployeeDetailsAsync();
        Task<InvUser> GetInvUserByCodeAsync(int code);
    }
}
