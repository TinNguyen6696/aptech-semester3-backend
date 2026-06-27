using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaLentShowcase.API.DTOS.Auth;
using TaLentShowcase.API.Models;
using TaLentShowcase.API.Services.Interfaces;

namespace TaLentShowcase.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<AuthResponse>.Ok(response, "Registration successful."));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(ApiResponse<AuthResponse>.Ok(response, "Login successful."));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var response = await _authService.RefreshAsync(request);
        return Ok(ApiResponse<AuthResponse>.Ok(response, "Token refreshed successfully."));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequest request)
    {
        var userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)!.Value;
        var expiration = long.Parse(User.FindFirst(JwtRegisteredClaimNames.Exp)!.Value);
        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expiration).UtcDateTime;

        await _authService.LogoutAsync(userId, jti, expiresAt, request.RefreshToken);
        return Ok(ApiResponse<object?>.Ok(null, "Logout successful."));
    }
}
