using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,                      // JS can never read this — blocks XSS token theft
            Secure = true,                        // only sent over HTTPS
            SameSite = SameSiteMode.None,          // needed because frontend (Netlify) and API (Render) are different domains
            Expires = DateTimeOffset.UtcNow.AddDays(7), // matches DB ExpiresAt
            Path = "/api/auth"                    // browser only attaches this cookie to auth endpoints, not every request
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (!result.IsSuccess)
            return HandleFailure(result);

        SetRefreshTokenCookie(result.Value!.RefreshToken); // put the RAW refresh token in a cookie

        // Access token still goes in the response body — the frontend keeps it
        // in memory (a JS variable), not localStorage, not a cookie.
        return Ok(new { token = result.Value.Token }); // send the access token back in the response body
    }

    [HttpPost("register-tenant")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterTenant([FromBody] RegisterTenantDto dto)
    {
        var result = await _authService.RegisterTenantAsync(dto);

        if (!result.IsSuccess)
            return HandleFailure(result);

        SetRefreshTokenCookie(result.Value!.RefreshToken); 
        return Ok(new { token = result.Value.Token });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var rawToken = Request.Cookies["refreshToken"]; // browser attached it, we just read it
        if (string.IsNullOrEmpty(rawToken))
            return Unauthorized();

        var result = await _authService.RefreshTokenAsync(rawToken);
        if (!result.IsSuccess)
            return HandleFailure(result);

        SetRefreshTokenCookie(result.Value!.RefreshToken);

        return Ok(new { token = result.Value.Token });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var rawToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(rawToken))
        {
            await _authService.LogoutAsync(rawToken);
        }

        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            Path = "/api/auth" // must match the Path used when the cookie was set,
                               // or the browser won't recognize it as the same cookie to delete
        });

        return Ok();
    }
}