using Microsoft.Data.SqlClient;
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

    public async Task<IEnumerable<Project>> GetAllAsync(Guid tenantId)
    {
        return await _context.Projects
            .FromSqlRaw("EXEC usp_GetProjectsByTenant {0}", tenantId)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(Guid id, Guid tenantId)
    {
        // Single entity lookup — stored proc for this comes in Phase 3.
        // For now, LINQ is fine here. Tenant isolation is still enforced.
        return await _context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == id
                               && p.TenantId == tenantId
                               && p.IsActive);
    }

    public async Task<Project> CreateAsync(Project project)
    {
        var idParam = new SqlParameter("@ProjectId", project.ProjectId);
        var tenantIdParam = new SqlParameter("@TenantId", project.TenantId);
        var nameParam = new SqlParameter("@Name", project.Name);
        var descriptionParam = new SqlParameter("@Description",
            project.Description ?? (object)DBNull.Value);
        var isActiveParam = new SqlParameter("@IsActive", project.IsActive);
        var createdAtParam = new SqlParameter("@CreatedAt", project.CreatedAt);
        var newIdParam = new SqlParameter("@NewId", System.Data.SqlDbType.UniqueIdentifier)
        {
            Direction = System.Data.ParameterDirection.Output
        };

        await _context.Database.ExecuteSqlRawAsync(
            "EXEC usp_CreateProject @ProjectId, @TenantId, @Name, @Description, @IsActive, @CreatedAt, @NewId OUTPUT",
            idParam, tenantIdParam, nameParam, descriptionParam,
            isActiveParam, createdAtParam, newIdParam);

        project.ProjectId = (Guid)newIdParam.Value;
        return project;
    }

    public async Task<Project?> UpdateAsync(Project project)
    {
        var existing = await _context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == project.ProjectId
                               && p.TenantId == project.TenantId
                               && p.IsActive);

        if (existing is null)
            return null;

        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid tenantId)
    {
        var existing = await _context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == id
                               && p.TenantId == tenantId
                               && p.IsActive);

        if (existing is null)
            return false;

        existing.IsActive = false;
        existing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}