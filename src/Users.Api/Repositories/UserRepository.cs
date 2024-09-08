using Microsoft.EntityFrameworkCore;
using Users.Api.Context;
using Users.Api.Models;

namespace Users.Api.Repositories
{
    public sealed class UserRepository(ApplicationDbContext context) : IUserRepository
    {

        /// .NET 8 New Feature -> Primary constructor -> context
        public async Task<bool> CreateAsync(User user, CancellationToken cancellationToken)
        {
            await context.AddAsync(user,cancellationToken);
            var result = await context.SaveChangesAsync(cancellationToken);
            return result > 0;

        }

        public async Task<bool> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            context.Remove(user);
            var result = await context.SaveChangesAsync(cancellationToken);
            return result > 0;

        }

        public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await context.Users.ToListAsync(cancellationToken);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await context.Users.FirstOrDefaultAsync(x=> x.Id == id, cancellationToken);
        }

        public async Task<bool> NameIsExist(string fullName, CancellationToken cancellationToken = default)
        {
            return await context.Users.AnyAsync(p => p.FullName == fullName, cancellationToken);
        }
    }
}
