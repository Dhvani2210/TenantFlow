using Microsoft.AspNetCore.Mvc;
using TenantFlow.Api.DTOs;
using TenantFlow.Api.Entities;
using TenantFlow.Api.Repositories.Interfaces;

namespace TenantFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectRepository _repository;

    public ProjectsController(IProjectRepository repository)
    {
        _repository = repository;
    }

    private static ProjectDto MapToDto(Project p) => new()
    {
        Id = p.ProjectId,
        TenantId = p.TenantId,
        Name = p.Name,
        Description = p.Description,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid tenantId)
    {
        var projects = await _repository.GetAllAsync(tenantId);
        var dtos = projects.Select(MapToDto);
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] Guid tenantId)
    {
        var project = await _repository.GetByIdAsync(id, tenantId);
        if (project is null) return NotFound();
        return Ok(MapToDto(project));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromQuery] Guid tenantId,
                                            [FromBody] CreateProjectDto dto)
    {
        // Controller maps DTO → entity before passing down.
        // In Phase 2 this mapping moves to the service layer.
        var project = new Project
        {
            ProjectId = Guid.NewGuid(),
            TenantId = tenantId,
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(project);
        return CreatedAtAction(nameof(GetById),
            new { id = created.ProjectId, tenantId },
            MapToDto(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromQuery] Guid tenantId,
                                            [FromBody] UpdateProjectDto dto)
    {
        // Map DTO → entity here. Repository receives a clean entity.
        var project = new Project
        {
            ProjectId = id,
            TenantId = tenantId,
            Name = dto.Name,
            Description = dto.Description
        };

        var updated = await _repository.UpdateAsync(project);
        if (updated is null) return NotFound();
        return Ok(MapToDto(updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid tenantId)
    {
        var deleted = await _repository.DeleteAsync(id, tenantId);
        if (!deleted) return NotFound();
        return NoContent();
    }

    
}