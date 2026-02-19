using Application.Interfaces.Models;

namespace Application.Interfaces.Contracts.Service
{
    public interface IEgxEmployeeService
    {
        Task<IReadOnlyList<EgxEmployeeDto>> GetAllEgxEmployeeAsync();
        Task<EgxEmployeeDto?> GetEgxEmployeeByIdAsync(int id);
        Task<int> CreateEgxEmployeeAsync(EgxEmployeeDto command);
        Task UpdateEgxEmployeeAsync(EgxEmployeeDto command);
        Task DeleteEgxEmployeeAsync(int id);
    }
}
