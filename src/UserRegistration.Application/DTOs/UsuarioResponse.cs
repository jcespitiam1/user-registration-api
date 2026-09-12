namespace UserRegistration.Application.DTOs;

public sealed class UsuarioResponse
{
    public int IdUsuario { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public int IdPais { get; init; }
    public string? PaisNombre { get; init; }
    public int IdDepartamento { get; init; }
    public string? DepartamentoNombre { get; init; }
    public int IdMunicipio { get; init; }
    public string? MunicipioNombre { get; init; }
    public string Direccion { get; init; } = string.Empty;
    public DateTimeOffset FechaRegistro { get; init; }
}
