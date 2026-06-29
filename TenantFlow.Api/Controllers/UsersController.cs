using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TenantFlow.Application.Common;
using TenantFlow.Application.DTOs;
using TenantFlow.Application.Interfaces;

namespace TenantFlow.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ApiBaseController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
        {
            var result = await _userService.GetAllAsync(paginationParams);
            if (!result.IsSuccess)
                return HandleFailure(result);
            return Ok(result.Value);
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);

            if (!result.IsSuccess)
                return HandleFailure(result);

            return Ok(result.Value);
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateAsync(dto);

            if (!result.IsSuccess)
                return HandleFailure(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Value!.UserId }, result.Value);
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            var result = await _userService.UpdateAsync(id, dto);

            if (!result.IsSuccess)
                return HandleFailure(result);

            return Ok(result.Value);
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteAsync(id);

            if (!result.IsSuccess)
                return HandleFailure(result);

            return NoContent();
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("invite")]
        public async Task<IActionResult> InviteMember([FromBody] InviteMemberDto dto)
        {
            var result = await _userService.InviteMemberAsync(dto);
            if (!result.IsSuccess)
                return HandleFailure(result);
            return Ok(result.Value);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
            {

            //var claims = User.Claims.Select(c => new { c.Type, c.Value });
            //return Ok(claims);
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _userService.ChangePasswordAsync(userId, dto);
            return result.IsSuccess ? NoContent() : HandleFailure(result);
        }
    }
}
