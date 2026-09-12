namespace UserRegistration.Domain.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public static NotFoundException For(string entity, object id) =>
        new($"{entity} con id '{id}' no fue encontrado.");
}
