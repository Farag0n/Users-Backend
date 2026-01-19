using System.ComponentModel.DataAnnotations;
using Users_Backend.Domain.Enums;

namespace Users_Backend.Application.DTOs;

public class UserUpdateDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string ConfirmPassword { get; set; }
}