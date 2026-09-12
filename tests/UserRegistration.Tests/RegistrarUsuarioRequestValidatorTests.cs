using UserRegistration.Application.DTOs;
using UserRegistration.Application.Validators;
using Xunit;

namespace UserRegistration.Tests;

public sealed class RegistrarUsuarioRequestValidatorTests
{
    private readonly RegistrarUsuarioRequestValidator _validator = new();

    private static RegistrarUsuarioRequest ValidRequest() => new(
        NumeroDocumento: "1020304050",
        Nombre: "Juan Camilo Espitia",
        Telefono: "+573001234567",
        IdPais: 1,
        IdDepartamento: 1,
        IdMunicipio: 1,
        Direccion: "Calle 123 # 45-67");

    [Fact]
    public void Request_valido_no_genera_errores()
    {
        var resultado = _validator.Validate(ValidRequest());

        Assert.True(resultado.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Juan123")]
    [InlineData("Juan_Camilo")]
    public void Nombre_invalido_genera_error(string nombre)
    {
        var request = ValidRequest() with { Nombre = nombre };

        var resultado = _validator.Validate(request);

        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, e => e.PropertyName == nameof(RegistrarUsuarioRequest.Nombre));
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("123-456-7890")]
    [InlineData("12345678901234567890")]
    public void Telefono_invalido_genera_error(string telefono)
    {
        var request = ValidRequest() with { Telefono = telefono };

        var resultado = _validator.Validate(request);

        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, e => e.PropertyName == nameof(RegistrarUsuarioRequest.Telefono));
    }

    [Theory]
    [InlineData("+573001234567")]
    [InlineData("3001234567")]
    public void Telefono_valido_no_genera_error(string telefono)
    {
        var request = ValidRequest() with { Telefono = telefono };

        var resultado = _validator.Validate(request);

        Assert.DoesNotContain(resultado.Errors, e => e.PropertyName == nameof(RegistrarUsuarioRequest.Telefono));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IdMunicipio_invalido_genera_error(int idMunicipio)
    {
        var request = ValidRequest() with { IdMunicipio = idMunicipio };

        var resultado = _validator.Validate(request);

        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, e => e.PropertyName == nameof(RegistrarUsuarioRequest.IdMunicipio));
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc12345")]
    [InlineData("1234")]
    [InlineData("1234567890123456")]
    public void NumeroDocumento_invalido_genera_error(string numeroDocumento)
    {
        var request = ValidRequest() with { NumeroDocumento = numeroDocumento };

        var resultado = _validator.Validate(request);

        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, e => e.PropertyName == nameof(RegistrarUsuarioRequest.NumeroDocumento));
    }
}
