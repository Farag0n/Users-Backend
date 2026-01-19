using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Users_Backend.Domain.Entities;
using Users_Backend.Domain.Interfaces;
using Users_Backend.Domain.ValueObjects;
using Users_Backend.Infrastructure.Data;

namespace Users_Backend.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserRepository> _logger;
    
    public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    //TODO mejorar los logs para cuando algo bueno o malo este pasando

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        try
        {
            return await _context.Users.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al obtener todos los usuarios");
            throw;
        }
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al obtener el usuario con ID {UserId}", userId);
            throw;
        }
    }

    public async Task<User?> GetUserByUserNameAsync(string userName)
    {
        try
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al obtener el usuario con UserName {UserName}", userName);
            throw;
        }
    }

    public async Task<User?> GetUserByEmailAsync(Email email)
    {
        try
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al obtener el usuario con Email {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<User>> GetDeletedUsersAsync()
    {
        try
        {
            return await _context.Users
                .Where(u => u.IsDeleted == true)
                .ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al obtener los usuarios eliminados");
            throw;
        }
    }

    public async Task<User?> CreateUserAsync(User user)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al crear el usuario con ID {UserId}", user.Id);
            throw;
        }
    }

    public async Task<User?> UpdateUserAsync(User user, Guid userId)
    {
        try
        {
            var existing = await _context.Users.FindAsync(user.Id);

            if (existing != null)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return user;
            }
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al actualizar el usuario con ID {UserId}", userId);
            throw;
        }
    }

    public async Task<User?> SoftDeleteUserAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user != null)
            {
                user.SoftDelete();
                await _context.SaveChangesAsync();
                return user;
            }
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al realizar soft delete del usuario con ID {UserId}", userId);
            throw;
        }
    }

    public async Task<User?> DeleteUserAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);

            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return user;  
            }
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al eliminar el usuario con ID {UserId}", userId);
            throw;
        }
    }
}