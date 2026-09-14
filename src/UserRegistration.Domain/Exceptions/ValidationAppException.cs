namespace UserRegistration.Domain.Exceptions;

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
