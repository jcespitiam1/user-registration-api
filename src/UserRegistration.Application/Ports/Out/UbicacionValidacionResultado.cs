namespace UserRegistration.Application.Ports.Out;

/// <summary>
/// Resultado de validar la coherencia referencial país -> departamento ->
/// municipio, tal como lo calcula el stored procedure sp_ubicacion_validar.
/// </summary>
public sealed class UbicacionValidacionResultado
{
    public bool PaisExiste { get; init; }
    public bool DepartamentoExiste { get; init; }
    public bool DepartamentoPerteneceAPais { get; init; }
    public bool MunicipioExiste { get; init; }
    public bool MunicipioPerteneceADepartamento { get; init; }
}
