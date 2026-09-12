namespace UserRegistration.Domain.Exceptions;

/// <summary>
/// Represents any validation failure that must surface as HTTP 400:
/// field-format errors as well as business/referential-coherence errors
/// (e.g. el municipio no pertenece al departamento indicado).
/// </summary>
public sealed class ValidationAppException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationAppException(string field, string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]> { [field] = [message] };
    }

    public ValidationAppException(IReadOnlyDictionary<string, string[]> errors)
        : base("Se encontraron errores de validación.")
    {
        Errors = errors;
    }
}
