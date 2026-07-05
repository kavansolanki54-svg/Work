using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace DallyWorkReoprt.DAL.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly IEntityType? _entityType;
        private readonly IKey? _primaryKey;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _entityType = _context.Model.FindEntityType(typeof(T));
            _primaryKey = _entityType?.FindPrimaryKey();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            Save();
        }

        public async Task AddAsync(T entity)
        {
            await _context.AddAsync(entity);
            await SaveAsync();
        }

        public void AddRange(IEnumerable<T> entity)
        {
            _dbSet.AddRange(entity);
            Save();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            Save();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await SaveAsync();
        }

        public void UpdateRange(IEnumerable<T> entity)
        {
            _dbSet.UpdateRange(entity);
            Save();
        }

        public void Remove(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                Save();
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IQueryable<T> GetByIdQueryable(int id)
        {
            if (_primaryKey == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} has no primary key.");

            var keyProperty = _primaryKey.Properties.FirstOrDefault();
            if (keyProperty == null)
                throw new InvalidOperationException($"No key property found for {typeof(T).Name}.");

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, keyProperty.Name);
            var constant = Expression.Constant(id);
            var predicate = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(predicate, parameter);

            return _dbSet.Where(lambda);
        }

        public T Add(T entity, bool skipSave)
        {
            _context.Add(entity);

            if (!skipSave)
            {
                Save();
            }

            return entity;
        }

        public T Update(T entity, bool skipSave)
        {
            _dbSet.Update(entity);

            if (!skipSave)
            {
                Save();
            }

            return entity;
        }
    }
}

