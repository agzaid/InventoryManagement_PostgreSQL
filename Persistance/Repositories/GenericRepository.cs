using Application.Interface.Contract.Persistance;
using Application.Interfaces.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistance.Repositories
{
    internal class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly InventoryManagementDbContext _dbContext;

        public GenericRepository(InventoryManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.AddAsync(entity);
            return entity;
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
        }
        public Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }
        public IQueryable<T> GetQueryable()
        {
            return _dbContext.Set<T>().AsQueryable();
        }
        public async Task<bool> Exists(int id)
        {
            var entity = await GetAsyncById(id);
            return entity != null;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        // In Persistance.Repositories/GenericRepository.cs

        public async Task<IReadOnlyList<T>> GetAllAsyncExpression<TKey>(
             Expression<Func<T, bool>>? filter = null,
             Expression<Func<T, TKey>>? orderBy = null,
             bool descending = false,
             string? includeProperties = null,
             bool tracked = false)
        {
            // 1. Initialize IQueryable and Tracking
            IQueryable<T> query = tracked ? _dbContext.Set<T>() : _dbContext.Set<T>().AsNoTracking();

            // 2. Apply Filter (WHERE clause)
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // 3. Apply Sorting (ORDER BY clause)
            if (orderBy != null)
            {
                query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }

            // 4. Apply Eager Loading (INCLUDE clause)
            if (!string.IsNullOrEmpty(includeProperties))
            {
                // Properties are passed as a comma-separated string (e.g., "Category,Supplier")
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // .Include() is used to load related data.
                    query = query.Include(includeProp.Trim());
                }
            }
            return await query.ToListAsync();
        }
        // 2. Get all products with a price over 100, no sorting, no includes, and tracked for updating later.
        //  var expensiveProducts = await _productRepository.GetAllAsyncExpression(
        //    filter: p => p.Price > 100,
        //    tracked: true
        //);

        public async Task<T> GetAsyncById(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }
        public async Task<PagedResult<T>> GetPagedAsync<TKey>(
      int pageNumber,
      int pageSize,
      Expression<Func<T, bool>>? filter = null,
      Expression<Func<T, TKey>>? orderBy = null,
      bool descending = false,
      string? includeProperties = null)
        {
            // 1. استخدام AsNoTracking لتحسين الأداء
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            // 2. التحقق من صحة المدخلات
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            if (filter != null)
                query = query.Where(filter);

            int totalCount = await query.CountAsync();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property.Trim());
                }
            }

            // 3. الترتيب الإلزامي: إذا لم يرسل المستخدم ترتيب، نستخدم خاصية افتراضية (مثلاً أول عمود)
            if (orderBy != null)
            {
                query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }
            // ملاحظة: إذا كنت تستخدم Oracle/SQL Server، قد تحتاج لترتيب افتراضي هنا لو كان orderBy نول

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task ExecuteRawSqlAsync(string sql, params object[] parameters)
        {
            await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);
        }
    }
}
