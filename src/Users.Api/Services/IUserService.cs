using Users.Api.DTOs;
using Users.Api.Models;

namespace Users.Api.Services
{
    public interface IUserService
    {
        public Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<bool> CreateAsync(CreateUserDto request, CancellationToken cancellationToken);
        public Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
