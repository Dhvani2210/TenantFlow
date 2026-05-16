using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenantFlow.Application.Common.Interfaces;
using TenantFlow.Application.DTOs;
using TenantFlow.Application.Interfaces;

namespace TenantFlow.Api.Controllers;


[AllowAnonymous]
[Route("api/auth")]
public class AuthController : ApiBaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return HandleFailure(result);
    }
}