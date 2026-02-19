using Application.Interfaces.Models;
using Application.Interfaces.Models.Pagination;

namespace Application.Interfaces.Contracts.Service
{
    public interface ISupplierService
    {
        Task<IReadOnlyList<SupplierDto>> GetAllSupplierAsync();
        Task<SupplierDto?> GetSupplierByIdAsync(int id);
        Task<int> CreateSupplierAsync(SupplierDto command);
        Task UpdateSupplierAsync(SupplierDto command, string oldCategory);
        Task DeleteSupplierAsync(string id);
        Task<PagedResult<SupplierDto>> GetSupplierPaginated(int page, string search, string category);
        Task<int> GetTotalCountAsync();
    }
}
