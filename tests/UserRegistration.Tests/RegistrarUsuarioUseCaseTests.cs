using FluentValidation;
using Moq;
using UserRegistration.Application.DTOs;
using UserRegistration.Application.Ports.Out;
using UserRegistration.Application.UseCases;
using UserRegistration.Application.Validators;
using UserRegistration.Domain.Exceptions;
using Xunit;

namespace UserRegistration.Tests;

public sealed class RegistrarUsuarioUseCaseTests
{
    private static RegistrarUsuarioRequest ValidRequest() => new(
        NumeroDocumento: "1020304050",
        Nombre: "Juan Camilo Espitia",
        Telefono: "+573001234567",
        IdPais: 1,
        IdDepartamento: 1,
        IdMunicipio: 1,
        Direccion: "Calle 123 # 45-67");

    private static UbicacionValidacionResultado UbicacionValida() => new()
    {
        PaisExiste = true,
        DepartamentoExiste = true,
        DepartamentoPerteneceAPais = true,
        MunicipioExiste = true,
        MunicipioPerteneceADepartamento = true
    };

    private static IValidator<RegistrarUsuarioRequest> RealValidator() => new RegistrarUsuarioRequestValidator();

    [Fact]
    public async Task Request_con_formato_invalido_no_consulta_ubicacion_ni_registra()
    {
        var ubicacionRepo = new Mock<IUbicacionRepository>(MockBehavior.Strict);
        var usuarioRepo = new Mock<IUsuarioRepository>(MockBehavior.Strict);
        var useCase = new RegistrarUsuarioUseCase(RealValidator(), ubicacionRepo.Object, usuarioRepo.Object);

        var request = ValidRequest() with { Telefono = "no-es-un-telefono" };

        await Assert.ThrowsAsync<ValidationAppException>(() => useCase.EjecutarAsync(request, CancellationToken.None));

        ubicacionRepo.VerifyNoOtherCalls();
        usuarioRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Municipio_que_no_pertenece_al_departamento_lanza_error_de_ubicacion()
    {
        var ubicacionRepo = new Mock<IUbicacionRepository>();
        ubicacionRepo
            .Setup(r => r.ValidarUbicacionAsync(1, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UbicacionValidacionResultado
            {
                PaisExiste = true,
                DepartamentoExiste = true,
                DepartamentoPerteneceAPais = true,
                MunicipioExiste = true,
                MunicipioPerteneceADepartamento = false
            });
        var usuarioRepo = new Mock<IUsuarioRepository>(MockBehavior.Strict);
        var useCase = new RegistrarUsuarioUseCase(RealValidator(), ubicacionRepo.Object, usuarioRepo.Object);

        var excepcion = await Assert.ThrowsAsync<ValidationAppException>(
            () => useCase.EjecutarAsync(ValidRequest(), CancellationToken.None));

        Assert.Contains(nameof(RegistrarUsuarioRequest.IdMunicipio), excepcion.Errors.Keys);
        usuarioRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Request_valido_con_ubicacion_coherente_registra_el_usuario()
    {
        var ubicacionRepo = new Mock<IUbicacionRepository>();
        ubicacionRepo
            .Setup(r => r.ValidarUbicacionAsync(1, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UbicacionValida());

        var respuestaEsperada = new UsuarioResponse { NumeroDocumento = "1020304050", Nombre = "Juan Camilo Espitia" };
        var usuarioRepo = new Mock<IUsuarioRepository>();
        usuarioRepo
            .Setup(r => r.RegistrarAsync(It.IsAny<RegistrarUsuarioRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(respuestaEsperada);

        var useCase = new RegistrarUsuarioUseCase(RealValidator(), ubicacionRepo.Object, usuarioRepo.Object);

        var resultado = await useCase.EjecutarAsync(ValidRequest(), CancellationToken.None);

        Assert.Equal("1020304050", resultado.NumeroDocumento);
        usuarioRepo.Verify(r => r.RegistrarAsync(It.IsAny<RegistrarUsuarioRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
