using Microsoft.EntityFrameworkCore;
using TenantFlow.Infrastructure.Persistence;
using TenantFlow.Domain.Entities;
using TenantFlow.Application.Interfaces;

namespace TenantFlow.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly TenantFlowDbContext _context;

    public ProjectRepository(TenantFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        // No TenantId filter needed here.
        // The global query filter in TenantFlowDbContext automatically appends
        // WHERE TenantId = <current tenant> to every query on this entity.
        return await _context.Projects
            .Where(p => p.IsActive).ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == id && p.IsActive);
    }

    public async Task<Project> CreateAsync(Project project)
    {
        // EF Core generates the INSERT automatically.
        // No manual SqlParameters needed.
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project?> UpdateAsync(Project project)
    {
        var existing = await _context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == project.ProjectId && p.IsActive);

        if (existing is null)
            return null;

        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == id && p.IsActive);

        if (existing is null)
            return false;

        existing.IsActive = false;
        existing.UpdatedAt = DateTime.UtcNow;

        var tasks = await _context.Tasks
        .Where(t => t.ProjectId == id && t.IsActive)
        .ToListAsync();
        foreach (var task in tasks)
            task.IsActive = false;

        await _context.SaveChangesAsync();
        return true;
    }
}