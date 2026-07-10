using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Data.Entities;

namespace TaskManager.Data.Repositories
{
    public interface IUserRepository : IRepository<Employee>
    {
        Task<Employee> FindByEmailAsync(string email);
        Task<Employee[]> ListActiveAsync();
    }

    public class UserRepository : Repository<Employee>, IUserRepository
    {
        public UserRepository(Func<AppDbContext> ctxFactory) : base(ctxFactory) { }

        public Task<Employee> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return Task.FromResult<Employee>(null);
            return Set.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<Employee[]> ListActiveAsync()
        {
            return await Set.Where(e => e.IsActive)
                            .OrderBy(e => e.FullName)
                            .ToArrayAsync();
        }
    }
}
