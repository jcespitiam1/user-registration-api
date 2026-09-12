namespace UserRegistration.Domain.Entities;

public sealed class Municipio
{
    public int IdMunicipio { get; init; }
    public int IdDepartamento { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
}
