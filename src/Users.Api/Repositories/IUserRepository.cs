using Users.Api.DTOs;
using Users.Api.Models;

namespace Users.Api.Repositories
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default);
        public Task<bool> NameIsExist(string fullName, CancellationToken cancellationToken = default);
        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        public Task<bool> CreateAsync(User user, CancellationToken cancellationToken = default);
        public Task<bool> DeleteAsync(User user, CancellationToken cancellationToken = default);
    }
}
