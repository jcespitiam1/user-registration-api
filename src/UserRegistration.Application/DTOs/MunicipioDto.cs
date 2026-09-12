namespace UserRegistration.Application.DTOs;

public sealed class MunicipioDto
{
    public int IdMunicipio { get; init; }
    public int IdDepartamento { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
}
