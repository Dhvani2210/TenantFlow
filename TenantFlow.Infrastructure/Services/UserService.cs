
using TenantFlow.Application.Common.Interfaces;
using TenantFlow.Application.DTOs;
using TenantFlow.Application.Interfaces;
using TenantFlow.Domain.Entities;
using TenantFlow.Application.Common;
using BCrypt.Net;


namespace TenantFlow.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITenantContext _tenantContext;

        public UserService(IUserRepository userRepository, ITenantContext tenantContext)
        {
            _userRepository = userRepository;
            _tenantContext = tenantContext;
        }

        private static UserDto MapToDto(User u) => new()
        {
            UserId = u.UserId,
            Email = u.Email,
            FullName = u.FullName,
            Role = u.Role,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
        };

        public async Task<Result<IEnumerable<UserDto>>> GetAllAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return Result<IEnumerable<UserDto>>.Success(users.Select(MapToDto));
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<UserDto>>.Failure(
                $"Failed to retrieve Users: {ex.Message}",
                ErrorType.ServerError);
            }
        }

        public async Task<Result<UserDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if(user is null)
                    return Result<UserDto>.Failure(
                    "User not found.",
                    ErrorType.NotFound);

                return Result<UserDto>.Success(MapToDto(user));
            }
            catch(Exception ex)
            {
                return Result<UserDto>.Failure(
                $"Failed to retrieve Users: {ex.Message}",
                ErrorType.ServerError);
            }
        }

        public async Task<Result<UserDto>> CreateAsync(CreateUserDto dto)
        {
            try
            {
                var emailExists = await _userRepository.GetByEmailAsync(dto.Email);
                if(emailExists is not null)
                    return Result<UserDto>.Failure(
                         $"Email already exists",
                         ErrorType.Conflict);

                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    FullName = dto.FullName,
                    Role = dto.Role,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    TenantId = _tenantContext.TenantId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var created = await _userRepository.CreateAsync(user);
                return Result<UserDto>.Success(MapToDto(created));
            }
            catch(Exception ex)
            {
                return Result<UserDto>.Failure(
                $"Failed to create User: {ex.Message}",
                ErrorType.ServerError);
            }
        }

        public async Task<Result<UserDto>> UpdateAsync(Guid id,UpdateUserDto dto)
        {
            try
            {
                var existing = await _userRepository.GetByIdAsync(id);

                if (existing is null)
                    return Result<UserDto>.Failure(
                        "User not found.",
                        ErrorType.NotFound);

                existing.FullName = dto.FullName;
                existing.Email = dto.Email;
                

                var updated = await _userRepository.UpdateAsync(existing);
                return Result<UserDto>.Success(MapToDto(updated!));
            }
            catch (Exception ex)
            {
                return Result<UserDto>.Failure(
                    $"Failed to update User: {ex.Message}",
                    ErrorType.ServerError);
            }
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var deleted = await _userRepository.DeleteAsync(id);

                if (!deleted)
                    return Result<bool>.Failure(
                        "User not found.",
                        ErrorType.NotFound);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(
                    $"Failed to delete User: {ex.Message}",
                    ErrorType.ServerError);
            }
        }
    }
}
