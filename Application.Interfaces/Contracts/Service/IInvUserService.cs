using Application.Interfaces.Models;

namespace Application.Interfaces.Contracts.Service
{
    public interface IInvUserService
    {
        Task<IReadOnlyList<InvUserDto>> GetAllInvUsersAsync();
        Task<InvUserDto?> GetInvUserByIdAsync(int id);
        Task<int> CreateInvUserAsync(InvUserDto command);
        Task UpdateInvUserAsync(InvUserDto command);
        Task DeleteInvUserAsync(int id);
    }
}
