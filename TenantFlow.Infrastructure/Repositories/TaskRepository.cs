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

    public async Task<PagedResult<Domain.Entities.Task>> GetAllAsync(Guid projectId, TaskQueryParams queryParams)
    {
        var query = _context.Tasks
            .Include(t => t.AssignedTo)
            .Where(t => t.ProjectId == projectId && t.IsActive);

        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            query = query.Where(t =>
                t.Name.Contains(queryParams.Search) ||
                t.Description.Contains(queryParams.Search));
        }

        if (queryParams.Status.HasValue)
        {
            query = query.Where(t => t.Status == queryParams.Status.Value);
        }

        var totalCount = await query.CountAsync();

        query = queryParams.SortBy?.ToLower() switch
        {
            "duedate" => queryParams.SortDirection?.ToLower() == "desc"
                ? query.OrderByDescending(t => t.DueDate)
                : query.OrderBy(t => t.DueDate),
            _ => query.OrderBy(t => t.CreatedAt)
        };

        var data = await query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        return new PagedResult<Domain.Entities.Task>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = queryParams.PageNumber,
            PageSize = queryParams.PageSize
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
        if (task.AssignedToUserId.HasValue)
        {
            await _context.Entry(task)
                .Reference(t => t.AssignedTo)
                .LoadAsync();
        }
        return task;
    }

    public async Task<Domain.Entities.Task?> UpdateAsync(Domain.Entities.Task task)
    {
        var existing = await _context.Tasks
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.TaskId == task.TaskId && t.IsActive);

        if (existing is null)
            return null;

        existing.Name = task.Name;
        existing.Description = task.Description;
        existing.DueDate = task.DueDate;
        existing.AssignedToUserId = task.AssignedToUserId;
        existing.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        // Re-load AssignedTo navigation property after saving
        // because EF Core doesn't auto-refresh it when AssignedToUserId changes
        await _context.Entry(existing)
            .Reference(t => t.AssignedTo)
            .LoadAsync();
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