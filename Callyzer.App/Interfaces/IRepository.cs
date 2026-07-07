using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Callyzer.App.Interfaces
{
    /// <summary>
    /// Generic repository interface defining standard CRUD operations.
    /// </summary>
    public interface IRepository<T> where T : class, new()
    {
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<T>> GetAllAsync(CancellationToken ct = default);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<int> InsertAsync(T entity, CancellationToken ct = default);
        Task<int> InsertAllAsync(IEnumerable<T> entities, CancellationToken ct = default);
        Task<int> UpdateAsync(T entity, CancellationToken ct = default);
        Task<int> DeleteAsync(T entity, CancellationToken ct = default);
        Task<int> DeleteByIdAsync(int id, CancellationToken ct = default);
        Task<int> CountAsync(CancellationToken ct = default);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<int> ExecuteAsync(string query, params object[] args);
    }
}
