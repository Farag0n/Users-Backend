using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders; //permite configurar entidades usando el patron Fluent API(tovia estoy aprendiendo que es esto)
using Users_Backend.Domain.Entities;
using Users_Backend.Domain.Enums;
using Users_Backend.Domain.ValueObjects;

namespace Users_Backend.Infrastructure.Data.Configurations;

//Esta clase define como se mapea la entidad a la base de datos
//implementa IEntityTypeConfiguration<entidad> para mantener el mapeo separado del DbContext
public class UserConfigurations :  IEntityTypeConfiguration<User>
{
    //Metodo que se ejecuta cuando EF construye la entidad en la db
    //se define y configura la entidad en la db
    public void Configure(EntityTypeBuilder<User> builder)
    {
        //Se define el nobre de la tabla(lo hace de forma automatica pero esto es una buena practica)
        builder.ToTable("Users");
        
        //define la clave primaria
        builder.HasKey(u => u.Id);
        
        //define un indice unico para username
        //asi no pueden existir dos usuarios con el mismo username
        builder.HasIndex(u => u.UserName).IsUnique();
        
        //configuracion de la propiedad username
        builder.Property(u => u.UserName)
            .IsRequired()//no es nuleable
            .HasMaxLength(100);//longitud maxima de la propiedad
        
        //configuracion del value object Email
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value, //conversion de dominio a db extrae el string interno del ValueObject
                value => new Email(value))//coversion de db a dominio reconstruye el ValueObject
            .IsRequired()
            .HasMaxLength(100);
        
        //configuracion del rol de usuario
        builder.Property(u => u.UserRole)
            .IsRequired()
            .HasMaxLength(100);
        
        //configuracion del hash de la contraseña
        builder.Property(u => u.PasswordHash)
            .IsRequired();
        
        //configracion del refresh token es opcional y tiene tamaño maximo
        builder.Property(u => u.RefreshToken)
            .HasMaxLength(500);
        
         builder.HasData(
         new
         {
             Id = Guid.Parse("d3e4f5a6-b7c8-4d9e-0f1a-2b3c4d5e6f7a"),
             Name = "Test",
             LastName = "Admin",
             Email = new Email("admin@qwe.com"),
             UserName = "admin",
             PasswordHash = "76ac3775bbadd88096d4aacef1ca7ac325fd1303dfac8f12d4462b3288cc5141\n",//pasword sin hashear: Adminqwe.123 
             UserRole = UserRole.Admin,
             CreateAt = DateTime.UtcNow,
             IsDeleted = false,
             RefreshTokenExpiresDate = DateTime.MinValue
         },
         new
         {
             Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
             Name = "Test",
             LastName = "User",
             Email = new Email("user@qwe.com"),
             UserName = "user",
             PasswordHash = "086736616e598710e984506abb07bb54d9f382103f145c9f1206247c2a61879d\n",//pasword sin hashear: Userqwe.123
             UserRole = UserRole.User,
             CreateAt = DateTime.UtcNow,
             IsDeleted = false,
             RefreshTokenExpiresDate = DateTime.MinValue
         });
    }
}