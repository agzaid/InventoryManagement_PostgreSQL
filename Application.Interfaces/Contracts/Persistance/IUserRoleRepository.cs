using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface IUserRoleRepository
    {
        Task<IList<UserRole>> GetRolesByUserIdAsync(string userId);
        Task<IList<UserRole>> GetUsersByRoleIdAsync(string roleId);
        Task AddUserRoleAsync(UserRole userRole);
        Task RemoveUserRoleAsync(string userId, string roleId);
        Task<bool> IsUserInRoleAsync(string userId, string roleId);
    }
}
