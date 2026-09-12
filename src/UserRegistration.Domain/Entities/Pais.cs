namespace UserRegistration.Domain.Entities;

public sealed class Pais
{
    public int IdPais { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
}
