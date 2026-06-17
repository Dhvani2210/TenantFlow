using TenantFlow.Application.Common;
using TenantFlow.Application.DTOs;

namespace TenantFlow.Application.Interfaces;

public interface IUserService
{
    Task<Result<IEnumerable<UserDto>>> GetAllAsync();
    Task<Result<UserDto>> GetByIdAsync(Guid id);
    Task<Result<UserDto>> CreateAsync(CreateUserDto dto);
    Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserDto dto);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<InviteMemberResponseDto>> InviteMemberAsync(InviteMemberDto dto);
    Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
}