using Microsoft.EntityFrameworkCore;
using TenantFlow.Api.Data;
using TenantFlow.Api.DTOs;
using TenantFlow.Api.Entities;
using TenantFlow.Api.Repositories.Interfaces;


namespace TenantFlow.Api.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly TenantFlowDbContext _context;

    public ProjectRepository(TenantFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync(Guid tenantId)
    {
    
        return await _context.Projects
            .Where(p => p.TenantId == tenantId && p.IsActive)
            .OrderBy(p => p.Name)
        
            .Select(p => new ProjectDto
            {
                Id = p.ProjectId,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                TenantId = p.TenantId
            })
            .ToListAsync(); 
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Projects
            .Where(p => p.ProjectId == id && p.TenantId == tenantId && p.IsActive)
            
            .Select(p => new ProjectDto
            {
                Id = p.ProjectId,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                TenantId = p.TenantId
            })
            .FirstOrDefaultAsync(); 
    }

    public async Task<ProjectDto> CreateAsync(Guid tenantId, CreateProjectDto dto)
    {
        var project = new Project
        {
            ProjectId = Guid.NewGuid(),
            TenantId = tenantId,       // Comes from the server, not the client.
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Projects.Add(project); 
        await _context.SaveChangesAsync(); 

        return new ProjectDto
        {
            Id = project.ProjectId,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            TenantId = project.TenantId
        };
    }


    public async Task<ProjectDto?> UpdateAsync(Guid id, Guid tenantId, UpdateProjectDto dto)
    {
  
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == id && p.TenantId == tenantId && p.IsActive);

        if (project is null)
            return null;

        project.Name = dto.Name;
        project.Description = dto.Description;
        await _context.SaveChangesAsync();

        return new ProjectDto
        {
            Id = project.ProjectId,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            TenantId = project.TenantId
        };
    }

    public async Task<bool> DeleteAsync(Guid id, Guid tenantId)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == id && p.TenantId == tenantId && p.IsActive);

        if (project is null)
            return false;

        // Soft delete — the row stays in the database.
        project.IsActive = false;

        await _context.SaveChangesAsync();
        return true;
    }
}