using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Generic repository interface providing standard CRUD operations for any entity.
    /// All repositories inherit from this base to ensure consistent data access patterns.
    /// </summary>
    /// <typeparam name="T">The entity type (must be a class with parameterless constructor).</typeparam>
    public interface IRepository<T> where T : class, new()
    {
        /// <summary>Retrieves an entity by its primary key.</summary>
        /// <param name="id">The primary key value.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The entity if found; otherwise null.</returns>
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>Retrieves all entities from the table.</summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of all entities.</returns>
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>Retrieves entities matching a predicate.</summary>
        /// <param name="predicate">Filter expression.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of matching entities.</returns>
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>Retrieves the first entity matching a predicate.</summary>
        /// <param name="predicate">Filter expression.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The first matching entity or null.</returns>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>Inserts a new entity.</summary>
        /// <param name="entity">The entity to insert.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of rows inserted.</returns>
        Task<int> InsertAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>Inserts multiple entities in a single transaction.</summary>
        /// <param name="entities">The entities to insert.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of rows inserted.</returns>
        Task<int> InsertAllAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        /// <summary>Updates an existing entity.</summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of rows updated.</returns>
        Task<int> UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>Deletes an entity.</summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of rows deleted.</returns>
        Task<int> DeleteAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>Deletes an entity by its primary key.</summary>
        /// <param name="id">The primary key value.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of rows deleted.</returns>
        Task<int> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>Returns the total count of entities.</summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The count of all entities.</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>Returns the count of entities matching a predicate.</summary>
        /// <param name="predicate">Filter expression.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The count of matching entities.</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>Checks if any entity matches a predicate.</summary>
        /// <param name="predicate">Filter expression.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if at least one entity matches.</returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>Executes a raw SQL query.</summary>
        /// <param name="query">The SQL query string.</param>
        /// <param name="args">Query parameters.</param>
        /// <returns>The number of affected rows.</returns>
        Task<int> ExecuteAsync(string query, params object[] args);
    }
}
