using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Data.Repositories
{
    public abstract class Repository<T> : IRepository<T>, IDisposable where T : class
    {
        protected readonly AppDbContext Context;
        protected readonly DbSet<T> Set;
        private bool _disposed;

        protected Repository(Func<AppDbContext> ctxFactory)
        {
            Context = ctxFactory();
            Set = Context.Set<T>();
        }

        public Task<T> GetByIdAsync(int id) => Set.FindAsync(id);

        public IQueryable<T> Query() => Set.AsQueryable();

        public void Add(T entity) => Set.Add(entity);

        public void Update(T entity) => Context.Entry(entity).State = EntityState.Modified;

        public void Remove(T entity) => Set.Remove(entity);

        public Task<int> SaveChangesAsync() => Context.SaveChangesAsync();

        public void Dispose()
        {
            if (_disposed) return;
            Context?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
