using FluentValidation;
using UserRegistration.Application.DTOs;

namespace UserRegistration.Application.Validators;

public sealed class RegistrarUsuarioRequestValidator : AbstractValidator<RegistrarUsuarioRequest>
{
    private const string NombreRegex = @"^[A-Za-zÀ-ÖØ-öø-ÿ]+(?:[ '\-][A-Za-zÀ-ÖØ-öø-ÿ]+)*$";
    private const string TelefonoRegex = @"^\+?[0-9]{7,15}$";
    private const string NumeroDocumentoRegex = @"^[0-9]{5,15}$";

    public RegistrarUsuarioRequestValidator()
    {
        RuleFor(x => x.NumeroDocumento)
            .NotEmpty().WithMessage("El número de documento es obligatorio.")
            .Matches(NumeroDocumentoRegex)
            .WithMessage("El número de documento debe tener entre 5 y 15 dígitos numéricos.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no debe superar 150 caracteres.")
            .Matches(NombreRegex).WithMessage("El nombre solo puede contener letras y espacios.");

        RuleFor(x => x.Telefono)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .Matches(TelefonoRegex)
            .WithMessage("El teléfono debe tener entre 7 y 15 dígitos, con un '+' opcional al inicio (ej: +573001234567).");

        RuleFor(x => x.IdPais)
            .GreaterThan(0).WithMessage("Debe indicar un país válido.");

        RuleFor(x => x.IdDepartamento)
            .GreaterThan(0).WithMessage("Debe indicar un departamento válido.");

        RuleFor(x => x.IdMunicipio)
            .GreaterThan(0).WithMessage("Debe indicar un municipio válido.");

        RuleFor(x => x.Direccion)
            .NotEmpty().WithMessage("La dirección es obligatoria.")
            .MaximumLength(250).WithMessage("La dirección no debe superar 250 caracteres.");
    }
}
