using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface IUserRoleRepository
    {
        Task<IList<UserRole>> GetRolesByUserIdAsync(int userId);
        Task<IList<UserRole>> GetUsersByRoleIdAsync(int roleId);
        Task AddUserRoleAsync(UserRole userRole);
        Task RemoveUserRoleAsync(int userId, int roleId);
        Task<bool> IsUserInRoleAsync(int userId, int roleId);
    }
}
