namespace UserRegistration.Domain.Entities;

public sealed class Usuario
{
    public int IdUsuario { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public int IdPais { get; init; }
    public int IdDepartamento { get; init; }
    public int IdMunicipio { get; init; }
    public string Direccion { get; init; } = string.Empty;
    public DateTimeOffset FechaRegistro { get; init; }
}
