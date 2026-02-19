using Application.Interfaces.Models;

namespace Application.Interfaces.Contracts.Service
{
    public interface IDepartmentService
    {
        Task<IReadOnlyList<DepartmentDto>> GetAllDepartmentsAsync();
        Task<DepartmentDto?> GetDepartmentByIdAsync(int id);
        Task<int> CreateDepartmentAsync(DepartmentDto command);
        Task UpdateDepartmentAsync(DepartmentDto command);
        Task DeleteDepartmentAsync(int id);
    }
}
