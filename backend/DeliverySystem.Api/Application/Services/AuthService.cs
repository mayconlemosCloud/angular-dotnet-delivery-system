using DeliverySystem.Api.Application.DTOs;
using DeliverySystem.Api.Domain.Interfaces;
using DeliverySystem.Api.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace DeliverySystem.Api.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, ITokenService tokenService, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var token = _tokenService.GenerateToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours);

        return new LoginResponse(token, expiresAt);
    }
}
