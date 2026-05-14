using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenantFlow.Application.DTOs;
using TenantFlow.Application.Interfaces;

namespace TenantFlow.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/projects/{projectId:guid}/tasks")]
    public class TasksController : ApiBaseController
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService) 
        { 
            _taskService = taskService;
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(Guid projectId)
        {
            var result = await _taskService.GetAllAsync(projectId);
            if (!result.IsSuccess)
                return HandleFailure(result);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, Guid projectId)
        {
            var result = await _taskService.GetByIdAsync(id, projectId);

            if (!result.IsSuccess)
                return HandleFailure(result);

            return Ok(result.Value);
        }

        [Authorize(Policy = "RequireManager")]
        [HttpPost]
        public async Task<IActionResult> Create( Guid projectId, [FromBody] CreateTaskDto dto)
        {
            var result = await _taskService.CreateAsync( projectId, dto);

            if (!result.IsSuccess)
                return HandleFailure(result);

            return CreatedAtAction(nameof(GetById), new { projectId, id = result.Value!.TaskId }, result.Value);
        }

        [Authorize(Policy = "RequireManager")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id,Guid projectId,  [FromBody] UpdateTaskDto dto)
        {
            var result = await _taskService.UpdateAsync(id, projectId, dto);

            if (!result.IsSuccess)
                return HandleFailure(result);

            return Ok(result.Value);
        }

        [Authorize(Policy = "RequireManager")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, Guid projectId)
        {
            var result = await _taskService.DeleteAsync(id, projectId);

            if (!result.IsSuccess)
                return HandleFailure(result);

            return NoContent();
        }

    }
}
