using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Users_Backend.Application.DTOs;
using Users_Backend.Application.Interfaces;
using Users_Backend.Domain.Entities;
using Users_Backend.Domain.ValueObjects;
using Users_Backend.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Users_Backend.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly TokenService _tokenService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository, 
        TokenService tokenService,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    //TODO Revisar todos los logs
    public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Obteniendo usuario por ID: {Id}", id);
            var user = await _userRepository.GetUserByIdAsync(id);
            return user == null ? null : MapToUserResponseDto(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario por ID {Id}", id);
            throw;
        }
    }

    public async Task<UserResponseDto?> GetUserByEmailAsync(string emailStr)
    {
        try
        {
            _logger.LogInformation("Buscando usuario por email: {Email}", emailStr);
            
            if (string.IsNullOrWhiteSpace(emailStr)) throw new ArgumentException("El email es requerido");
            
            var emailVo = new Email(emailStr);
            
            var user = await _userRepository.GetUserByEmailAsync(emailVo);
            return user == null ? null : MapToUserResponseDto(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar usuario por email {Email}", emailStr);
            throw;
        }
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToUserResponseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los usuarios");
            throw;
        }
    }
    
    public async Task<UserResponseDto?> CreateUserAsync(UserRegisterDto userCreateDto)
    {
        try
        {
            _logger.LogInformation("Creando usuario: {Email}", userCreateDto.Email);
            
            var emailVo = new Email(userCreateDto.Email);
            var existingUser = await _userRepository.GetUserByEmailAsync(emailVo);
            
            if (existingUser != null)
                throw new InvalidOperationException("El email ya está registrado");
            
            var existingByUsername = await _userRepository.GetUserByUserNameAsync(userCreateDto.UserName);
            if (existingByUsername != null)
                throw new InvalidOperationException("El nombre de usuario ya está en uso");
            
            var user = new User(
                userCreateDto.Name,
                userCreateDto.LastName,
                userCreateDto.Email, 
                userCreateDto.UserName,
                HashPassword(userCreateDto.Password),
                userCreateDto.Role,
                false
            );

            var createdUser = await _userRepository.CreateUserAsync(user);
            return MapToUserResponseDto(createdUser!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            throw;
        }
    }
    
    public async Task<UserResponseDto?> UpdateUserAsync(Guid userId, UserUpdateDto userUpdateDto)
    {
        try
        {
            _logger.LogInformation("Actualizando usuario ID: {Id}", userId);
        
            var existingUser = await _userRepository.GetUserByIdAsync(userId);
            if (existingUser == null) return null;
        
            // Validar email único
            if (existingUser.Email.Value != userUpdateDto.Email)
            {
                var emailVo = new Email(userUpdateDto.Email);
                var userWithSameEmail = await _userRepository.GetUserByEmailAsync(emailVo);
                if (userWithSameEmail != null && userWithSameEmail.Id != userId)
                    throw new InvalidOperationException("El email ya está en uso");
            
                existingUser.ChangeEmail(userUpdateDto.Email);
            }
        
            // Validar username único
            if (existingUser.UserName != userUpdateDto.UserName)
            {
                var userWithSameUsername = await _userRepository.GetUserByUserNameAsync(userUpdateDto.UserName);
                if (userWithSameUsername != null && userWithSameUsername.Id != userId)
                    throw new InvalidOperationException("El nombre de usuario ya está en uso");
            }
        
            // Actualizar datos básicos
            existingUser.UpdateUser(
                userUpdateDto.Name, 
                userUpdateDto.LastName, 
                userUpdateDto.UserName
            );
        
            // Cambiar contraseña si se proporciona
            if (!string.IsNullOrEmpty(userUpdateDto.Password))
            {
                existingUser.ChangePassword(HashPassword(userUpdateDto.Password));
            }
        
            var updatedUser = await _userRepository.UpdateUserAsync(existingUser, userId);
            return updatedUser == null ? null : MapToUserResponseDto(updatedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<UserResponseDto>> GetDeletedUsersAsync()
    {
        try
        {
            var users = await _userRepository.GetDeletedUsersAsync();
            return users.Select(MapToUserResponseDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al obtener los usuarios eliminados");
            throw;
        }
    }

    public async Task<UserResponseDto?> SoftDeleteUserAsync(Guid id)
    {
        try
        {
            var deletedUser = await _userRepository.SoftDeleteUserAsync(id);
            return deletedUser == null ? null : MapToUserResponseDto(deletedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario ID {Id}", id);
            throw;
        }
    }
    
    public async Task<UserResponseDto?> DeleteUserAsync(Guid id)
    {
        try
        {
            var deletedUser = await _userRepository.DeleteUserAsync(id);
            return MapToUserResponseDto(deletedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario ID {Id}", id);
            throw;
        }
    }
    
    public async Task<UserResponseDto?> GetUserByUserNameAsync(string userName)
    {
        try
        {
            _logger.LogInformation("Buscando usuario por username: {UserName}", userName);

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("El nombre de usuario es requerido");

            var user = await _userRepository.GetUserByUserNameAsync(userName);

            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado");

            return MapToUserResponseDto(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar usuario por username {UserName}", userName);
            throw;
        }
    }

    //TODO Refactorizar este metodo para que no sea tan largo para mejorar aplicando buenas practicas
    public async Task<(string AccessToken, string RefreshToken)> AuthenticateAsync(UserLoginDto loginDto)
    {
        try
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                throw new ArgumentException("Email y contraseña requeridos");
            
            var emailVo = new Email(loginDto.Email);
            var user = await _userRepository.GetUserByEmailAsync(emailVo);

            if (user == null || user.PasswordHash != HashPassword(loginDto.Password))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }
            
            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email.Value, user.UserRole.ToString());
            var refreshToken = _tokenService.GenerateRefreshToken();
            var days = _tokenService.GetRefreshTokenExpirationDays();
            
            user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(days));
            await _userRepository.UpdateUserAsync(user, user.Id);

            return (accessToken, refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en autenticación");
            throw;
        }
    }

    //TODO Refactorizar este metodo para que no sea tan largo para mejorar aplicando buenas practicas
    public async Task<(string AccessToken, string RefreshToken)> RegisterAsync(UserRegisterDto registerDto)
    {
        try 
        {
            var emailVo = new Email(registerDto.Email);
            if (await _userRepository.GetUserByEmailAsync(emailVo) != null)
                throw new InvalidOperationException("El usuario ya existe");
            
            var user = new User(
                registerDto.Name,
                registerDto.LastName,
                registerDto.Email, 
                registerDto.UserName,
                HashPassword(registerDto.Password),
                registerDto.Role,
                false
            );

            await _userRepository.CreateUserAsync(user);
        
            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email.Value, user.UserRole.ToString());
            var refreshToken = _tokenService.GenerateRefreshToken();
            var days = _tokenService.GetRefreshTokenExpirationDays();
        
            user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(days));
            await _userRepository.UpdateUserAsync(user, user.Id);
        
            return (accessToken, refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registro");
            throw;
        }
    }
    
    public async Task<(string NewAccessToken, string NewRefreshToken)> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        try
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim)) throw new SecurityTokenException("Token inválido");
            
            User? user;
            
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                user = await _userRepository.GetUserByIdAsync(userId);
            }
            else
            {
                user = null;
            }
            
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiresDate <= DateTime.UtcNow)
                throw new SecurityTokenException("Token inválido o expirado");

            var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email.Value, user.UserRole.ToString());
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var days = _tokenService.GetRefreshTokenExpirationDays();
            
            user.UpdateRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(days));
            await _userRepository.UpdateUserAsync(user, user.Id);

            return (newAccessToken, newRefreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refrescando token");
            throw;
        }
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }
    
    private UserResponseDto MapToUserResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email.Value,
            UserName = user.UserName,
            Role = user.UserRole,
            Password = user.PasswordHash,
            IsDeleted = user.IsDeleted
        };
    }
}