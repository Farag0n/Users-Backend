using Microsoft.EntityFrameworkCore; //EF Core es el ORM que me permite mapear clases/entidades a estructuras en la db
using Users_Backend.Domain.Entities;
using Users_Backend.Infrastructure.Data.Configurations; //archivos de configuracion de mapeo de cada entidad

namespace Users_Backend.Infrastructure.Data;

//DbContext es la clase central de EF que permite
//- guardar cambios
//- construir consultas de mejor manera
//- rastrear entidades para construir tablas
public class AppDbContext : DbContext
{
    //constructor que recibe las opciones de configuracion del db context
    //estas opciones se configuran en Extensions para mantener la responsabilidad de Infrastructure
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
    
    //representacion de la entidad como tabla en la base de datos
    public DbSet<User> Users { get; set; }

    //metodo que se ejecuta cuando EF construye el modelo(que modelo?)
    //aca se define como se mapean las entidades, las claves primarias, las relaciones
    //y las conversiones de value objects
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Llama a la implementacion base de EF necesario para no perder configuraciones internas de EF
        base.OnModelCreating(modelBuilder);
        
        //se aplica la configuracion de cada entidad con su respectivo archivo
        modelBuilder.ApplyConfiguration(new UserConfigurations());
    }
}