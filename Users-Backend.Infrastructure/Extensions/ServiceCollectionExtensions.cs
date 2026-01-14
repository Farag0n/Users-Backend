using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;// Contiene IServiceCollection y métodos para registrar dependencias
using Users_Backend.Domain.Interfaces;
using Users_Backend.Infrastructure.Data;
using Users_Backend.Infrastructure.Repositories;

namespace Users_Backend.Infrastructure.Extensions;

// Clase estática que contiene métodos de extensión
// para registrar servicios de infraestructura
public static class ServiceCollectionExtensions
{
    // Método de extensión para IServiceCollection
    // Permite registrar toda la infraestructura con una sola línea en program,
    //ejmplo: builder.Services.AddInfrastructure(builder.Configuration);
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //se obtiene la cadena de coneccion desde el appsetings
        string? conn = configuration.GetConnectionString("DefaultConnection");
    
        //Registro del DbContext en el contenedor de dependencias (un objeto que sabe como crear, cuando y que implementar. contenedor(IserviceCollection))
        //se configura EF para usar mysql
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                conn,
                ServerVersion.AutoDetect(conn)
            )
        );
        
        //Registro de repositorios
        //Se vincula la interfaz de dominio con la implementacion de infrastructura
        // se hace el registro aca y no en program por encapsulamiento ya que la capa de API
        //No deberia saber los detalles de infrastructura, solo que necesita infrastructura
        //Tambien para desacoplar la API de la base de datos con esto si mañana se cambia la db
        //no hay que tocar program y por ultimo  para limpieza y legibilidad
        //asi no se tinen un program con un monton de:
        //builder.Services.AddScoped<IUserRepository, UserRepository>();
        //scoped significa que crea una instancia por peticion HTTP
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}