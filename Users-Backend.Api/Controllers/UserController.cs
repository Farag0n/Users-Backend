using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users_Backend.Application.DTOs;
using Users_Backend.Application.Interfaces;

namespace Users_Backend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // GET: api/User
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        try 
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo usuarios");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // GET: api/User/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        // Validación de seguridad: ¿Es el propio usuario o es un admin?
        var currentUserId = GetCurrentUserId();
        if (!User.IsInRole("Admin") && currentUserId != id)
        {
            return Forbid();//TODO ponerle un authorize
        }

        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { Message = "Usuario no encontrado" });
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo usuario {Id}", id);
            return BadRequest(new { Message = ex.Message });
        }
    }

    // GET: api/User/username/{username}
    [HttpGet("username/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByUserName(string username)
    {
        try
        {
            var user = await _userService.GetUserByUserNameAsync(username);
            return Ok(user);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "Usuario no encontrado" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    // GET: api/User/email/{email}
    [HttpGet("email/{email}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        try
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null) return NotFound(new { Message = "Usuario no encontrado" });
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    // POST: api/User
    // Este método permite a un ADMIN crear un usuario manualmente.
    // Reutilizamos UserRegisterDto porque contiene todos los datos necesarios (pass, nombre, rol, etc.)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] UserRegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Usamos CreateUserAsync (que devuelve UserResponseDto) en lugar de RegisterAsync (que devuelve Tokens)
            var createdUser = await _userService.CreateUserAsync(registerDto);
            
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }
        catch (InvalidOperationException ex) // Ej: Email duplicado
        {
            return Conflict(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando usuario");
            return BadRequest(new { Message = ex.Message });
        }
    }

    // PUT: api/User/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto userUpdateDto)
    {
        if (id != userUpdateDto.Id)
            return BadRequest(new { Message = "El ID de la URL no coincide con el cuerpo" });

        // Seguridad: Solo Admin o el mismo usuario pueden editar
        var currentUserId = GetCurrentUserId();
        if (!User.IsInRole("Admin") && currentUserId != id)
        {
            return Forbid();
        }

        try
        {
            var updatedUser = await _userService.UpdateUserAsync(userUpdateDto);
            if (updatedUser == null) return NotFound(new { Message = "Usuario no encontrado" });

            return Ok(updatedUser);
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

    // DELETE: api/User/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var deletedUser = await _userService.DeleteUserAsync(id);
            if (deletedUser == null) return NotFound(new { Message = "Usuario no encontrado" });

            return Ok(new { Message = "Usuario eliminado correctamente", User = deletedUser });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error eliminando usuario {Id}", id);
            return BadRequest(new { Message = ex.Message });
        }
    }
    
    // DELETE: api/User/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SoftDelete(Guid id)
    {
        try
        {
            var deletedUser = await _userService.SoftDeleteUserAsync(id);
            if (deletedUser == null) return NotFound(new { Message = "Usuario no encontrado" });

            return Ok(new { Message = "Usuario eliminado correctamente", User = deletedUser });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error eliminando usuario {Id}", id);
            return BadRequest(new { Message = ex.Message });
        }
    }

    // Método privado para obtener el ID del usuario logueado desde el Token JWT
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (Guid.TryParse(userIdClaim, out Guid userId))
        {
            return userId;
        }
        
        return Guid.Empty;
    }
}