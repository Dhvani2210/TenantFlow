using Microsoft.AspNetCore.Mvc;
using TenantFlow.Api.DTOs;
using TenantFlow.Api.Repositories.Interfaces;

namespace TenantFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectRepository _projectRepository;

    public ProjectsController(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    // GET api/projects?tenantId=...
    // TenantId hardcoded as a query param for now, we will move
    // this into a JWT claim where it belongs.
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid tenantId)
    {
        var projects = await _projectRepository.GetAllAsync(tenantId);
        return Ok(projects);
    }

    // GET api/projects/{id}?tenantId=...
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] Guid tenantId)
    {
        var project = await _projectRepository.GetByIdAsync(id, tenantId);

        if (project is null)
            return NotFound();

        return Ok(project);
    }

    // POST api/projects?tenantId=...
    [HttpPost]
    public async Task<IActionResult> Create([FromQuery] Guid tenantId, [FromBody] CreateProjectDto dto)
    {
        var created = await _projectRepository.CreateAsync(tenantId, dto);

        return CreatedAtAction(nameof(GetById), new { id = created.Id, tenantId }, created);
    }

    // PUT api/projects/{id}?tenantId=...
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromQuery] Guid tenantId, [FromBody] UpdateProjectDto dto)
    {
        var updated = await _projectRepository.UpdateAsync(id, tenantId, dto);

        // Null means either the project doesn't exist or it belongs
        // to a different tenant — we return 404 for both, intentionally.
        
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    // DELETE api/projects/{id}?tenantId=...
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid tenantId)
    {
        var deleted = await _projectRepository.DeleteAsync(id, tenantId);

        if (!deleted)
            return NotFound();

        // 204 No Content is the correct HTTP response for a successful delete.
        // The resource is gone — there's nothing to return.
        return NoContent();
    }
}