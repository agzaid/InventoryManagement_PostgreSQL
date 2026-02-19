using Application.Interfaces.Models;

namespace Application.Interfaces.Contracts.Service
{
    public interface ISystemManagementService
    {
        Task<SysMangementIndexDto> GetAllEgxEmployeesAsync();
        Task<int> CreateInvUserAsync(InvUserDto command);
        Task DeleteEgxEmployeeAsync(int id);
        Task<int> UpdateInvUserAsync(InvUserDto command);
        Task<int> DeleteInvUserAsync(int command);
        Task<(bool Success, string Message)> RelayDataAsync(int storeCode, DateTime systemDate);
    }
}
