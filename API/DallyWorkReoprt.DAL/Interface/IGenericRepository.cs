namespace DallyWorkReoprt.DAL.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        Task<T> GetByIdAsync(int id);
        void Add(T entity);
        T Add(T entity, bool skipSave);
        Task AddAsync(T entity);
        void Update(T entity);
        Task UpdateAsync(T entity);
        void Remove(int id);
        void Save();
        Task SaveAsync();
        void AddRange(IEnumerable<T> entity);
        void UpdateRange(IEnumerable<T> entity);
        T Update(T entity, bool skipSave);
    }
}

