using Users_Backend.Domain.Enums;

namespace Users_Backend.Application.DTOs;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
    public bool IsDeleted { get; set; }
}