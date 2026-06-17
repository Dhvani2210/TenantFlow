using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TenantFlow.Application.Common;
using TenantFlow.Application.DTOs;
using TenantFlow.Application.Interfaces;
using TenantFlow.Infrastructure.Persistence;

namespace TenantFlow.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IValidator<LoginDto> _validator;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly TenantFlowDbContext _context;
    private readonly IValidator<RegisterTenantDto> _registerValidator;
    

    public AuthService( IValidator<LoginDto> validator, IUserRepository userRepository,
                IConfiguration configuration, TenantFlowDbContext context, IValidator<RegisterTenantDto> registerValidator)
    {
        _validator = validator;
        _userRepository = userRepository;
        _configuration = configuration;
        _context = context;
        _registerValidator = registerValidator;
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

    public async Task<Result<LoginResponseDto>> RegisterTenantAsync(RegisterTenantDto dto)
    {
        var validation = await _registerValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result<LoginResponseDto>.Failure(
                string.Join(", ", validation.Errors.Select(e => e.ErrorMessage)),
                ErrorType.Validation);

        var existingUser = await _userRepository.GetByEmailForAuthAsync(dto.Email);
        if (existingUser is not null)
            return Result<LoginResponseDto>.Failure("Email already registered.", ErrorType.Validation);

        try
        {
            var tenant = new Domain.Entities.Tenant
            {
                TenantId = Guid.NewGuid(),
                Name = dto.CompanyName,
                Email = dto.Email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var user = new Domain.Entities.User
            {
                UserId = Guid.NewGuid(),
                TenantId = tenant.TenantId,
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = Domain.Enums.Role.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Tenants.AddAsync(tenant);
            await _userRepository.CreateAsync(user);
            await _context.SaveChangesAsync();

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
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("FullName", user.FullName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
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