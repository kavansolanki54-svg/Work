using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using Callyzer.App.Exceptions;
using Callyzer.App.Interfaces;
using Callyzer.App.SQLite;

namespace Callyzer.App.Repositories
{
    /// <summary>
    /// Generic base repository implementing standard CRUD operations using SQLite-net-pcl.
    /// All domain-specific repositories inherit from this base class.
    /// </summary>
    public class BaseRepository<T> : IRepository<T> where T : class, new()
    {
        protected readonly DatabaseService _dbService;
        protected SQLiteAsyncConnection Db => _dbService.Database;

        public BaseRepository(DatabaseService dbService)
        {
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        }

        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                return await Db.FindAsync<T>(id);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to get {typeof(T).Name} by ID {id}", "GetById", typeof(T).Name, ex);
            }
        }

        public virtual async Task<List<T>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                return await Db.Table<T>().ToListAsync();
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to get all {typeof(T).Name}", "GetAll", typeof(T).Name, ex);
            }
        }

        public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                return await Db.Table<T>().Where(predicate).ToListAsync();
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to find {typeof(T).Name}", "Find", typeof(T).Name, ex);
            }
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var results = await Db.Table<T>().Where(predicate).Take(1).ToListAsync();
                return results.Count > 0 ? results[0] : null;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to find first {typeof(T).Name}", "FirstOrDefault", typeof(T).Name, ex);
            }
        }

        public virtual async Task<int> InsertAsync(T entity, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                return await Db.InsertAsync(entity);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to insert {typeof(T).Name}", "Insert", typeof(T).Name, ex);
            }
        }

        public virtual async Task<int> InsertAllAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                return await Db.InsertAllAsync(entities);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to insert multiple {typeof(T).Name}", "InsertAll", typeof(T).Name, ex);
            }
        }

        public virtual async Task<int> UpdateAsync(T entity, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                return await Db.UpdateAsync(entity);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to update {typeof(T).Name}", "Update", typeof(T).Name, ex);
            }
        }

        public virtual async Task<int> DeleteAsync(T entity, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                return await Db.DeleteAsync(entity);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to delete {typeof(T).Name}", "Delete", typeof(T).Name, ex);
            }
        }

        public virtual async Task<int> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                return await Db.DeleteAsync<T>(id);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to delete {typeof(T).Name} by ID {id}", "DeleteById", typeof(T).Name, ex);
            }
        }

        public virtual async Task<int> CountAsync(CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                return await Db.Table<T>().CountAsync();
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to count {typeof(T).Name}", "Count", typeof(T).Name, ex);
            }
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                return await Db.Table<T>().Where(predicate).CountAsync();
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to count filtered {typeof(T).Name}", "Count", typeof(T).Name, ex);
            }
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var count = await Db.Table<T>().Where(predicate).CountAsync();
                return count > 0;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to check existence of {typeof(T).Name}", "Any", typeof(T).Name, ex);
            }
        }

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
