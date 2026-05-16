using TenantFlow.Domain.Entities;

namespace TenantFlow.Application.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user);
    Task<User?> UpdateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByEmailForAuthAsync(string email);
}