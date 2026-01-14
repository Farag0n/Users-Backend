namespace Users_Backend.Domain.ValueObjects;

//El sealed significa que la clase no puede ser heredada (es final)
//esto se usa en los ValueObjects para evitar que se pueda modificar el comportamiento
public sealed class Email
{
    //Propiedad publica que almacena el valor del Email
    //solo tiene getter para que una vez creado no se pueda cambiar
    public string Value { get; }

    //Constructor que recibe un string como parámetro para crear un Email válido
    public Email(string value)
    {
         //Regla del objeto para validar que no sea nulo, esté vacío o contenga solo espacios
         if (string.IsNullOrWhiteSpace(value))
             throw new ArgumentException("Email vacío o nulo");

         //Regla del objeto para validar que no le falta el @
         if (!value.Contains("@"))
             throw new ArgumentException("Email inválido");
         
         //Regla del objeto para validar que el email tenga un dominio válido (.)
         if (!value.Contains("."))
             throw new ArgumentException("Email invalido");
        
         //Asigna el valor interno del Email una sola vez en el constructor,
        //haciendo que el objeto sea inmutable
         Value = value;
    }
     
    //Retorna un booleano.
    //Dos objetos Email son iguales si representan el mismo valor (Value),
    //aunque sean instancias distintas en memoria
    public override bool Equals(object? obj) => obj is Email other && Value == other.Value;

    //Retorna un número (hash) calculado a partir del Value.
    //Emails con el mismo Value devolverán el mismo hash,
    //lo que permite que colecciones como HashSet o Dictionary
    //detecten correctamente objetos iguales.
    public override int GetHashCode() => Value.GetHashCode();
    
    //📌 ¿Qué es el hash?
    // Un número que el sistema usa para agrupar y buscar objetos rápidamente. -J.A.R.V.I.S
}