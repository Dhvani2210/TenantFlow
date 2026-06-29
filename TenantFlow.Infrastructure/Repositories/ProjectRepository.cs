using Microsoft.EntityFrameworkCore;
using TenantFlow.Application.Common;
using TenantFlow.Application.Interfaces;
using TenantFlow.Domain.Entities;
using TenantFlow.Infrastructure.Persistence;

namespace TenantFlow.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly TenantFlowDbContext _context;

    public ProjectRepository(TenantFlowDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Project>> GetAllAsync(PaginationParams paginationParams)
    {
        // No TenantId filter needed here.
        // The global query filter in TenantFlowDbContext automatically appends
        // WHERE TenantId = <current tenant> to every query on this entity.

        var query = _context.Projects.Where(p => p.IsActive);

        var totalCount = await query.CountAsync();

        var data = await query
            .OrderBy(p => p.CreatedAt)
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return new PagedResult<Project>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        };
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