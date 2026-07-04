using Microsoft.EntityFrameworkCore;
using TenantFlow.Application.Common;
using TenantFlow.Application.Interfaces;
using TenantFlow.Domain.Entities;
using TenantFlow.Infrastructure.Persistence;

namespace TenantFlow.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TenantFlowDbContext _context;

    public UserRepository(TenantFlowDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<User>> GetAllAsync(PaginationParams paginationParams)
    {
        var query = _context.Users.Where(u => u.IsActive);

        var totalCount = await query.CountAsync();

        var data = await query
            .OrderBy(u => u.CreatedAt)
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return new PagedResult<User>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        };
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateAsync(User user)
    {
        var existing = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == user.UserId && u.IsActive);

        if (existing is null)
            return null;

        existing.Email = user.Email;
        existing.FullName = user.FullName;
       

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);

        if (existing is null)
            return false;

        existing.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLower() && u.IsActive);
    }

    public async Task<User?> GetByEmailForAuthAsync(string email)
    {
        return await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email.ToLower() && u.IsActive);
    }
}