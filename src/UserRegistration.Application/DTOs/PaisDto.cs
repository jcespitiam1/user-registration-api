namespace UserRegistration.Application.DTOs;

public sealed class PaisDto
{
    public int IdPais { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
}
