using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenantFlow.Application.Common;
using TenantFlow.Application.DTOs;
using TenantFlow.Application.Interfaces;

namespace TenantFlow.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ApiBaseController
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
    {
        var result = await _projectService.GetAllAsync(paginationParams);

        if (!result.IsSuccess)
            return HandleFailure(result);
        return Ok(result.Value);
    }


    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _projectService.GetByIdAsync(id);

        if (!result.IsSuccess)
            return HandleFailure(result);

        return Ok(result.Value);
    }


    [Authorize(Policy = "RequireAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
    {
        var result = await _projectService.CreateAsync(dto);

        if (!result.IsSuccess)
            return HandleFailure(result);

        // CreatedAtAction sets the 201 status and builds the Location header
        // pointing to the GetById route for the newly created project.
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }


    [Authorize(Policy = "RequireManager")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDto dto)
    {
        var result = await _projectService.UpdateAsync(id, dto);

        if (!result.IsSuccess)
            return HandleFailure(result);

        return Ok(result.Value);
    }

    [Authorize(Policy = "RequireAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _projectService.DeleteAsync(id);

        if (!result.IsSuccess)
            return HandleFailure(result);

        // 204 No Content — success, but nothing to return.
        return NoContent();
    }
}