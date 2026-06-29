using TenantFlow.Application.Common;
using TenantFlow.Domain.Entities;

namespace TenantFlow.Application.Interfaces;

public interface IUserRepository
{
    Task<PagedResult<User>> GetAllAsync(PaginationParams paginationParams);
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user);
    Task<User?> UpdateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByEmailForAuthAsync(string email);
}