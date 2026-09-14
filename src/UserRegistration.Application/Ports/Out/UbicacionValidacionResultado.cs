namespace UserRegistration.Application.Ports.Out;

public sealed class UbicacionValidacionResultado
{
    public bool PaisExiste { get; init; }
    public bool DepartamentoExiste { get; init; }
    public bool DepartamentoPerteneceAPais { get; init; }
    public bool MunicipioExiste { get; init; }
    public bool MunicipioPerteneceADepartamento { get; init; }
}
