using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TenantFlow.Application.Common;
using TenantFlow.Application.DTOs;
using TenantFlow.Application.Interfaces;

namespace TenantFlow.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IValidator<LoginDto> _validator;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService( IValidator<LoginDto> validator, IUserRepository userRepository,
                             IConfiguration configuration)
    {
        _validator = validator;
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result<LoginResponseDto>.Failure(
                string.Join(", ", validation.Errors.Select(e => e.ErrorMessage)),
                ErrorType.Validation);

        var user = await _userRepository.GetByEmailForAuthAsync(dto.Email);
        if (user is null)
            return Result<LoginResponseDto>.Failure("Invalid credentials.", ErrorType.Unauthorized);


        var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!passwordValid)
            return Result<LoginResponseDto>.Failure("Invalid credentials.", ErrorType.Unauthorized);

        try
        {
            var token = GenerateToken(user);
            return Result<LoginResponseDto>.Success(new LoginResponseDto { Token = token });
        }
        catch (Exception ex)
        {
            return Result<LoginResponseDto>.Failure(ex.Message, ErrorType.ServerError);
        }
    }

    private string GenerateToken(Domain.Entities.User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim("TenantId", user.TenantId.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
    }
}