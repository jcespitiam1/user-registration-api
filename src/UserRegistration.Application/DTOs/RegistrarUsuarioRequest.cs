namespace UserRegistration.Application.DTOs;

public sealed record RegistrarUsuarioRequest(
    string Nombre,
    string Telefono,
    int IdPais,
    int IdDepartamento,
    int IdMunicipio,
    string Direccion);
