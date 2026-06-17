using TenantFlow.Application.DTOs;
using TenantFlow.Application.Common;

namespace TenantFlow.Application.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(LoginDto dto);
    Task<Result<LoginResponseDto>> RegisterTenantAsync(RegisterTenantDto dto);
}