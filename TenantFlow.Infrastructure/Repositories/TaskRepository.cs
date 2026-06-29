using Microsoft.EntityFrameworkCore;
using TenantFlow.Application.Common;
using TenantFlow.Application.Interfaces;
using TenantFlow.Domain.Entities;
using TenantFlow.Infrastructure.Persistence;

namespace TenantFlow.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TenantFlowDbContext _context;

    public TaskRepository(TenantFlowDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Domain.Entities.Task>> GetAllAsync(Guid projectId, PaginationParams paginationParams)
    {
        var query = _context.Tasks
            .Include(t => t.AssignedTo)
            .Where(t => t.ProjectId == projectId && t.IsActive);

        var totalCount = await query.CountAsync();

        var data = await query
            .OrderBy(t => t.CreatedAt)
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return new PagedResult<Domain.Entities.Task>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        };
    }


    public async Task<Domain.Entities.Task?> GetByIdAsync(Guid id, Guid projectId)
    {
        return await _context.Tasks
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.TaskId == id && t.ProjectId == projectId && t.IsActive);
    }

    public async Task<Domain.Entities.Task> CreateAsync(Domain.Entities.Task task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<Domain.Entities.Task?> UpdateAsync(Domain.Entities.Task task)
    {
        var existing = await _context.Tasks
            .FirstOrDefaultAsync(t => t.TaskId == task.TaskId && t.IsActive);

        if (existing is null)
            return null;

        existing.Name = task.Name;
        existing.Description = task.Description;
        existing.DueDate = task.DueDate;
        existing.AssignedToUserId = task.AssignedToUserId;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid projectId)
    {
        var existing = await _context.Tasks
            .FirstOrDefaultAsync(t => t.TaskId == id && t.ProjectId == projectId && t.IsActive);

        if (existing is null)
            return false;

        existing.IsActive = false;
        existing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}