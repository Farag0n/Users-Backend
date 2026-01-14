using Users_Backend.Domain.Entities;
using Users_Backend.Domain.ValueObjects;

namespace Users_Backend.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByUserNameAsync(string userName);
    Task<User?> GetUserByEmailAsync(Email email);
    Task<User?> CreateUserAsync(User user);
    Task<User?> UpdateUserAsync(User user, Guid userId);
    Task<User?> SoftDeleteUserAsync(Guid userId);
    Task<User> DeleteUserAsync(Guid userId);
}