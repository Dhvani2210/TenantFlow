using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TenantFlow.Application.Common;
using TenantFlow.Application.DTOs;
using TenantFlow.Application.Interfaces;
using TenantFlow.Domain.Entities;
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

    private static string GenerateRawRefreshToken()
    {
        // 64 random bytes -> long, unguessable string
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToBase64String(bytes);
    }

    private async Task<string> IssueRefreshTokenAsync(Guid userId)
    {
        var rawToken = GenerateRawRefreshToken();

        var refreshToken = new RefreshToken
        {
            RefreshTokenId = Guid.NewGuid(),
            UserId = userId,
            TokenHash = HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return rawToken; // caller sends this raw value to the client — DB only ever kept the hash
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
            var accessToken = GenerateToken(user);
            var refreshToken = await IssueRefreshTokenAsync(user.UserId);

            return Result<LoginResponseDto>.Success(new LoginResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken
            });
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
                Email = dto.Email.ToLower(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var user = new Domain.Entities.User
            {
                UserId = Guid.NewGuid(),
                TenantId = tenant.TenantId,
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
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
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<Result<LoginResponseDto>> RefreshTokenAsync(string rawToken)
    {
        var tokenHash = HashToken(rawToken);  // recompute the hash of what came in

        var existing = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash); // does this hash match a stored one?


        if (existing is null || !existing.IsActive)
            return Result<LoginResponseDto>.Failure("Invalid or expired refresh token.", ErrorType.Unauthorized);

        existing.RevokedAt = DateTime.UtcNow; // Rotation: kill the one just used, right away

        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.UserId == existing.UserId && u.IsActive);
        if (user is null)
            return Result<LoginResponseDto>.Failure("User not found.", ErrorType.Unauthorized);

        var newAccessToken = GenerateToken(user);
        var newRefreshToken = await IssueRefreshTokenAsync(user.UserId);

        await _context.SaveChangesAsync(); // persists the RevokedAt change on `existing`

        return Result<LoginResponseDto>.Success(new LoginResponseDto
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }


    public async Task<Result> LogoutAsync(string rawToken)
    {
        var tokenHash = HashToken(rawToken);

        var existing = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash);

        if (existing is not null)
        {
            existing.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // Always succeed, even if the token wasn't found — logging out of
        // an already-invalid session shouldn't be an error the user sees.
        return Result.Success();
    }
}