using Users_Backend.Application.DTOs;
using Users_Backend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Users_Backend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        try
        {
            var (accessToken, refreshToken) = await _userService.AuthenticateAsync(loginDto);
            
            return Ok(new 
            { 
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Message = "Login exitoso" 
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
    {
        try
        {
            var (accessToken, refreshToken) = await _userService.RegisterAsync(registerDto);
            
            return Ok(new 
            { 
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Message = "Registro exitoso" 
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
    {
        try
        {
            var (newAccessToken, newRefreshToken) = await _userService.RefreshTokenAsync(
                refreshTokenDto.AccessToken, 
                refreshTokenDto.RefreshToken
            );
            
            return Ok(new 
            { 
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Message = "Tokens refrescados exitosamente" 
            });
        }
        catch (SecurityTokenException ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
    }
}