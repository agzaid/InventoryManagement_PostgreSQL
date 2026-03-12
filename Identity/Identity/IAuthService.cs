using Application.Interfaces.Models.Identity;
using Domain.Entities;

namespace Identity.Identity
{
    public interface IAuthService
    {
        Task<ApplicationUser?> AuthenticateAsync(string username, string password);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<bool> IsUserInRoleAsync(string userId, string roleName);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
