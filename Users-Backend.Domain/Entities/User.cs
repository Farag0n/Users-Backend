using Users_Backend.Domain.Enums;
using Users_Backend.Domain.ValueObjects;

namespace Users_Backend.Domain.Entities;

public class User
{
    //El private set se usa para mantener la integridad y seguridad de los datos
    //y obliga a usar un constructor para crear un usuario.
    //Sin el private ser cualquier parte del codigo puede hacer lo que quiera con la informacion
    //lo que genera un gran problema de seguridad
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string LastName { get; private set; } 
    public Email Email { get; private set; }//Se usa el Value Object
    public string UserName { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreateAt { get; private set; } = DateTime.UtcNow;
    public UserRole UserRole { get; private set; }
    public bool IsDeleted { get; private set; }
    
    public string? RefreshToken { get; private set; }
    public DateTime RefreshTokenExpiresDate { get; private set; }
    
    //Constructor vacio para EF (sin esto se vuelve Hiroshima)
    protected User() {}

    //Constructor publico 
    public User(string name, string lastName, string email, string userName, string passwordHash, UserRole userRole, bool isDeleted)
    {
        //Validaciones básicas
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nombre requerido");
        if(string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Apellido requerido");
        if(string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Usuario requerido");
        if(string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Contraseña requerida");

        Id = Guid.NewGuid();
        Name = name;
        LastName = lastName;
        Email = new Email(email);// Se instancia el ValueObject si esta mal el mismo lanza la ecepcion 
        UserName = userName;
        PasswordHash = passwordHash;
        UserRole = userRole;
        CreateAt = DateTime.UtcNow;
        IsDeleted = false; 
    }
    
    //Metodos de domino para la logica de negocio del mismo y exponer el set con su respetivo metodo
    
    public void UpdateUser(string name, string lastName, string userName)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nombre requerido");
        if(string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Apellido requerido");
        if(string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("UserName requerido");
        Name = name;
        LastName = lastName;
        UserName = userName;
    }

    public void ChangePassword(string newPasswordHash)
    {
        if(string.IsNullOrWhiteSpace(newPasswordHash)) throw new ArgumentException("Hash inválido");
        PasswordHash = newPasswordHash;
    }
    
    public void SoftDelete()
    {
        if (IsDeleted) return;
        IsDeleted = true;
    }
    
    public void UpdateRefreshToken(string token, DateTime expires)
    {
        RefreshToken = token;
        RefreshTokenExpiresDate = expires;
    }
    
    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiresDate = DateTime.UtcNow;
    }
    
    public void ChangeEmail(string newEmail)
    {
        this.Email = new Email(newEmail);
    }
    
    
}