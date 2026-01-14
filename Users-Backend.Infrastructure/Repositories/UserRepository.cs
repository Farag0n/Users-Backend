using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Users_Backend.Domain.Entities;
using Users_Backend.Domain.Interfaces;
using Users_Backend.Domain.ValueObjects;
using Users_Backend.Infrastructure.Data;

namespace Users_Backend.Infrastructure.Repositories;

//logica de como intetactua la db con la entidd
public class UserRepository : IUserRepository
{
    //inyeccion de dependencias por constructor
    private readonly AppDbContext _context;
    private readonly ILogger<UserRepository> _logger;
    public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

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
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetUserByUserNameAsync(string userName)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<User?> GetUserByEmailAsync(Email email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateUserAsync(User user, Guid userId)
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

    public async Task<User?> SoftDeleteUserAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        if (user != null)
        {
            // Soft delete
            user.SoftDelete();
            
            await _context.SaveChangesAsync();
            return user;
        }
        return null;
    }

    public async Task<User> DeleteUserAsync(Guid userId)
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
}