using Users_Backend.Application.DTOs;
using Users_Backend.Domain.ValueObjects;

namespace Users_Backend.Application.Interfaces;

public interface IUserService
{
    Task<UserResponseDto?> GetUserByIdAsync(Guid id);
    Task<UserResponseDto?> GetUserByEmailAsync(string emailStr);
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto?> UpdateUserAsync(Guid userId, UserUpdateDto updateUserDto);
    Task<UserResponseDto?> GetUserByUserNameAsync(string userName);
    Task<IEnumerable<UserResponseDto>> GetDeletedUsersAsync();
    Task<UserResponseDto?> SoftDeleteUserAsync(Guid id);
    Task<UserResponseDto?> DeleteUserAsync(Guid id);
    Task<UserResponseDto?> CreateUserAsync(UserRegisterDto registerDto);
    
    //retorna AccessToken y RefreshToken
    Task<(string AccessToken, string RefreshToken)> AuthenticateAsync(UserLoginDto userLoginDto);
    Task<(string AccessToken, string RefreshToken)> RegisterAsync(UserRegisterDto userRegisterDto);
    
    //Metodo para refrescar el token
    Task<(string NewAccessToken, string NewRefreshToken)> RefreshTokenAsync(string accessToken, string refreshToken);
}