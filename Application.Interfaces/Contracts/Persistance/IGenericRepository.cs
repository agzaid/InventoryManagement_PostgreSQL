using Application.Interfaces.Models.Pagination;
using System.Linq.Expressions;

namespace Application.Interface.Contract.Persistance
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetAsyncById(int id);
        Task<T> AddAsync(T entity);
        Task<bool> Exists(int id);
        Task UpdateAsync(T entity);
        IQueryable<T> GetQueryable();
        Task DeleteAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task<IReadOnlyList<T>> GetAllAsyncExpression<TKey>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, TKey>>? orderBy = null,
            bool descending = false,
            string? includeProperties = null,
            bool tracked = false);
        Task<PagedResult<T>> GetPagedAsync<TKey>(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, TKey>>? orderBy = null,
        bool descending = false,
        string? includeProperties = null);
        Task ExecuteRawSqlAsync(string sql, params object[] parameters);
    }
}
