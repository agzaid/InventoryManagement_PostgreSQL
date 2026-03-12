using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.Contracts.Persistance
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task<ApplicationUser?> GetByEmailAsync(string email);
    }
}
