using FluentValidation;
using UserRegistration.Application.Common;
using UserRegistration.Application.DTOs;
using UserRegistration.Application.Ports.In;
using UserRegistration.Application.Ports.Out;
using UserRegistration.Domain.Exceptions;

namespace UserRegistration.Application.UseCases;

public sealed class RegistrarUsuarioUseCase : IRegistrarUsuarioUseCase
{
    private readonly IValidator<RegistrarUsuarioRequest> _validator;
    private readonly IUbicacionRepository _ubicacionRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public RegistrarUsuarioUseCase(
        IValidator<RegistrarUsuarioRequest> validator,
        IUbicacionRepository ubicacionRepository,
        IUsuarioRepository usuarioRepository)
    {
        _validator = validator;
        _ubicacionRepository = ubicacionRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioResponse> EjecutarAsync(RegistrarUsuarioRequest request, CancellationToken cancellationToken)
    {
        var formatValidation = await _validator.ValidateAsync(request, cancellationToken);
        if (!formatValidation.IsValid)
        {
            throw new ValidationAppException(formatValidation.ToErrorDictionary());
        }

        var ubicacion = await _ubicacionRepository.ValidarUbicacionAsync(
            request.IdPais, request.IdDepartamento, request.IdMunicipio, cancellationToken);

        var errores = ConstruirErroresDeUbicacion(ubicacion);
        if (errores.Count > 0)
        {
            throw new ValidationAppException(errores);
        }

        return await _usuarioRepository.RegistrarAsync(request, cancellationToken);
    }

    private static Dictionary<string, string[]> ConstruirErroresDeUbicacion(UbicacionValidacionResultado ubicacion)
    {
        var errores = new Dictionary<string, string[]>();

        if (!ubicacion.PaisExiste)
        {
            errores[nameof(RegistrarUsuarioRequest.IdPais)] = ["El país indicado no existe."];
            return errores;
        }

        if (!ubicacion.DepartamentoExiste)
        {
            errores[nameof(RegistrarUsuarioRequest.IdDepartamento)] = ["El departamento indicado no existe."];
            return errores;
        }

        if (!ubicacion.DepartamentoPerteneceAPais)
        {
            errores[nameof(RegistrarUsuarioRequest.IdDepartamento)] =
                ["El departamento indicado no pertenece al país seleccionado."];
            return errores;
        }

        if (!ubicacion.MunicipioExiste)
        {
            errores[nameof(RegistrarUsuarioRequest.IdMunicipio)] = ["El municipio indicado no existe."];
            return errores;
        }

        if (!ubicacion.MunicipioPerteneceADepartamento)
        {
            errores[nameof(RegistrarUsuarioRequest.IdMunicipio)] =
                ["El municipio indicado no pertenece al departamento seleccionado."];
        }

        return errores;
    }
}
