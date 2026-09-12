namespace UserRegistration.Application.DTOs;

public sealed class DepartamentoDto
{
    public int IdDepartamento { get; init; }
    public int IdPais { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
}
