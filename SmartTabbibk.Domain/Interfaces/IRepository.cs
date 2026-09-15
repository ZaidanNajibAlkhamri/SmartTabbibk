namespace SmartTabbibk.Domain.Interfaces
{
    /// <summary>
    /// عمليات CRUD الأساسية المشتركة بين كل الـ Repositories
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
    }
}
