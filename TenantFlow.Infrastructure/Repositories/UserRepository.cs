using Microsoft.EntityFrameworkCore;
using TenantFlow.Application.Interfaces;
using TenantFlow.Infrastructure.Persistence;
using TenantFlow.Domain.Entities;

namespace TenantFlow.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TenantFlowDbContext _context;

    public UserRepository(TenantFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        // Global query filter scopes by TenantId automatically.
        // Only active users are returned.
        return await _context.Users
            .Where(u => u.IsActive)
            .ToListAsync();
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
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }
}