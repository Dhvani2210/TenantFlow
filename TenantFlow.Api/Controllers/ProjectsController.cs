using Microsoft.AspNetCore.Mvc;
using TenantFlow.Application.DTOs;
using TenantFlow.Domain.Entities;
using TenantFlow.Application.Interfaces;
using TenantFlow.Application.Common.Interfaces;

namespace TenantFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectRepository _repository;
    private readonly ITenantContext _tenantContext;

    public ProjectsController(IProjectRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }



    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var projects = await _repository.GetAllAsync(_tenantContext.TenantId);
        var dtos = projects.Select(ProjectMappings.ToDto);
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var project = await _repository.GetByIdAsync(id, _tenantContext.TenantId);
        if (project is null) return NotFound();
        return Ok(ProjectMappings.ToDto(project));
    }

    [HttpPost]
    public async Task<IActionResult> Create( [FromBody] CreateProjectDto dto)
    {
        // Controller maps DTO → entity before passing down.
        // In Phase 2 this mapping moves to the service layer.
        var project = new Project
        {
            ProjectId = Guid.NewGuid(),
            TenantId = _tenantContext.TenantId,
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(project);
        return CreatedAtAction(nameof(GetById),
            new { id = created.ProjectId, _tenantContext.TenantId },
            ProjectMappings.ToDto(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDto dto)
    {
        // Map DTO → entity here. Repository receives a clean entity.
        var project = new Project
        {
            ProjectId = id,
            TenantId = _tenantContext.TenantId,
            Name = dto.Name,
            Description = dto.Description
        };

        var updated = await _repository.UpdateAsync(project);
        if (updated is null) return NotFound();
        return Ok(ProjectMappings.ToDto(updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _repository.DeleteAsync(id, _tenantContext.TenantId);
        if (!deleted) return NotFound();
        return NoContent();
    }

    
}