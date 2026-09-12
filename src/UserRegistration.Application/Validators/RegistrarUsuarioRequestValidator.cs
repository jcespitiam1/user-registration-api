using FluentValidation;
using UserRegistration.Application.DTOs;

namespace UserRegistration.Application.Validators;

/// <summary>
/// Valida el FORMATO de los campos de entrada. La coherencia referencial
/// (que el municipio pertenezca al departamento y al país indicados) es
/// responsabilidad del caso de uso, ya que requiere consultar la base de
/// datos y por tanto no es una regla de formato.
/// </summary>
public sealed class RegistrarUsuarioRequestValidator : AbstractValidator<RegistrarUsuarioRequest>
{
    private const string NombreRegex = @"^[A-Za-zÀ-ÖØ-öø-ÿ]+(?:[ '\-][A-Za-zÀ-ÖØ-öø-ÿ]+)*$";
    private const string TelefonoRegex = @"^\+?[0-9]{7,15}$";

    public RegistrarUsuarioRequestValidator()
    {
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
