using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using DailyTaskSheet.App.Exceptions;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.SQLite;

namespace DailyTaskSheet.App.Repositories
{
    /// <summary>
    /// Generic base repository implementing standard CRUD operations using SQLite-net-pcl.
    /// All domain-specific repositories inherit from this base class.
    /// </summary>
    /// <typeparam name="T">The entity type (must be a class with parameterless constructor).</typeparam>
    public class BaseRepository<T> : IRepository<T> where T : class, new()
    {
        /// <summary>The database service providing the SQLite connection.</summary>
        protected readonly DatabaseService _dbService;

        /// <summary>The SQLite async connection.</summary>
        protected SQLiteAsyncConnection Db => _dbService.Database;

        /// <summary>
        /// Initializes a new instance of <see cref="BaseRepository{T}"/>.
        /// </summary>
        /// <param name="dbService">The database service.</param>
        public BaseRepository(DatabaseService dbService)
        {
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        }

        /// <inheritdoc />
        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await Db.FindAsync<T>(id);
            }
            catch (global::System.OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to get {typeof(T).Name} by ID {id}", "GetById", typeof(T).Name, ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await Db.Table<T>().ToListAsync();
            }
            catch (global::System.OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to get all {typeof(T).Name}", "GetAll", typeof(T).Name, ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await Db.Table<T>().Where(predicate).ToListAsync();
            }
            catch (global::System.OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to find {typeof(T).Name}", "Find", typeof(T).Name, ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var results = await Db.Table<T>().Where(predicate).Take(1).ToListAsync();
                return results.Count > 0 ? results[0] : null;
            }
            catch (global::System.OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to find first {typeof(T).Name}", "FirstOrDefault", typeof(T).Name, ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> InsertAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await Db.InsertAsync(entity);
            }
            catch (global::System.OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to insert {typeof(T).Name}", "Insert", typeof(T).Name, ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> InsertAllAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await Db.InsertAllAsync(entities);
            }
            catch (global::System.OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to insert multiple {typeof(T).Name}", "InsertAll", typeof(T).Name, ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await Db.UpdateAsync(entity);
            }
            catch (global::System.OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to update {typeof(T).Name}", "Update", typeof(T).Name, ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await Db.DeleteAsync(entity);
            }
            catch (global::System.OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to delete {typeof(T).Name}", "Delete", typeof(T).Name, ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await Db.DeleteAsync<T>(id);
            }
            catch (global::System.OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to delete {typeof(T).Name} by ID {id}", "DeleteById", typeof(T).Name, ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await Db.Table<T>().CountAsync();
            }
            catch (global::System.OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to count {typeof(T).Name}", "Count", typeof(T).Name, ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await Db.Table<T>().Where(predicate).CountAsync();
            }
            catch (global::System.OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to count filtered {typeof(T).Name}", "Count", typeof(T).Name, ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var count = await Db.Table<T>().Where(predicate).CountAsync();
                return count > 0;
            }
            catch (global::System.OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to check existence of {typeof(T).Name}", "Any", typeof(T).Name, ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> ExecuteAsync(string query, params object[] args)
        {
            try
            {
                return await Db.ExecuteAsync(query, args);
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to execute query on {typeof(T).Name}", "Execute", typeof(T).Name, ex);
            }
        }
    }
}
