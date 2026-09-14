# API de Registro de Usuarios

Prueba técnica: servicio en **.NET 8 / C#** para registrar usuarios (número de
documento, nombre, teléfono, país, departamento, municipio y dirección) sobre
**PostgreSQL**, consumiendo la base de datos exclusivamente a través de
**stored procedures**.

El **número de documento** es la llave primaria natural de `usuario` (no un
id autoincremental sin significado): identifica a la persona en el mundo real
y es lo que se usa en la URL para consultarla (`GET /api/usuarios/{numeroDocumento}`).

## Arquitectura

El proyecto sigue **arquitectura hexagonal (puertos y adaptadores)**:

```
src/
├── UserRegistration.Domain          Núcleo: excepciones de dominio (ValidationAppException,
│                                     NotFoundException, ConflictException). Sin dependencias externas.
├── UserRegistration.Application     Casos de uso + puertos.
│   ├── Ports/In                     Puertos de entrada (interfaces de los casos de uso).
│   ├── Ports/Out                    Puertos de salida (interfaces que debe implementar la infraestructura).
│   ├── UseCases                     Implementación de los puertos de entrada.
│   ├── Validators                   Validación de FORMATO (FluentValidation).
│   └── DTOs
├── UserRegistration.Infrastructure  Adaptador secundario (driven): Postgres + Dapper,
│                                     implementando los puertos de salida. Solo llama SPs.
└── UserRegistration.Api             Adaptador primario (driving): Controllers, middleware
                                      de errores y Program.cs como composition root.
    └── wwwroot                      Interfaz web (HTML/CSS/JS vanilla) servida como
                                      archivos estáticos, mismo origen que la API.

tests/
├── UserRegistration.Tests             Pruebas unitarias (validador + caso de uso, con mocks).
└── UserRegistration.IntegrationTests  Pruebas de integración contra un Postgres real
                                        (Testcontainers) y contra la API completa
                                        (WebApplicationFactory).

database/
├── 01_create_tables.sql             Esquema: pais, departamento, municipio, usuario.
├── 02_seed_data.sql                 Datos base (Colombia, México, Perú).
└── 03_stored_procedures.sql         Todos los stored procedures usados por la API.
```

### Dos niveles de validación

1. **Formato** (`RegistrarUsuarioRequestValidator`, FluentValidation): número de
   documento `^[0-9]{5,15}$`, nombre solo letras/espacios, teléfono
   `^\+?[0-9]{7,15}$`, ids de ubicación positivos, longitudes máximas.
2. **Coherencia referencial** (`RegistrarUsuarioUseCase` + `sp_ubicacion_validar`):
   verifica que el país exista, el departamento exista y pertenezca a ese país, y
   que el municipio exista y pertenezca a ese departamento — antes de insertar.

Ambos casos producen `ValidationAppException`, traducida por el middleware a
`400 Bad Request` con `ValidationProblemDetails` (errores por campo).

### Manejo de errores

`ExceptionHandlingMiddleware` centraliza la traducción de excepciones a
respuestas `application/problem+json` consistentes:

| Excepción                  | HTTP |
|-----------------------------|------|
| `ValidationAppException`    | 400  |
| `NotFoundException`         | 404  |
| `ConflictException` (documento duplicado, FK/unique en Postgres) | 409 |
| Cualquier otra              | 500 (sin detalles internos) |

### Patrones de diseño usados

- **Puertos y adaptadores (hexagonal)** para desacoplar el dominio de Postgres/HTTP.
- **Repository** para encapsular el acceso a los stored procedures.
- **Dependency Injection** vía composition root (`Program.cs` + `AddApplication` / `AddInfrastructure`).
- **Middleware / Chain of Responsibility** para el manejo centralizado de errores.

## Cómo ejecutar

### Con Docker (recomendado)

```bash
docker compose up --build
```

- Interfaz web: http://localhost:18080/
- Swagger: http://localhost:18080/swagger
- PostgreSQL queda expuesto en `localhost:5434` (`postgres`/`postgres`, db `registro_usuarios`).

Los scripts de `database/` se ejecutan automáticamente la primera vez que se
crea el volumen de datos de Postgres (`docker-entrypoint-initdb.d`).

### Local (sin Docker)

Requiere .NET 8 SDK y una instancia de PostgreSQL accesible.

```bash
psql -h localhost -U postgres -f database/01_create_tables.sql
psql -h localhost -U postgres -f database/02_seed_data.sql
psql -h localhost -U postgres -f database/03_stored_procedures.sql

dotnet run --project src/UserRegistration.Api
```

Ajusta `ConnectionStrings:Postgres` en `src/UserRegistration.Api/appsettings.json`
o mediante la variable de entorno `ConnectionStrings__Postgres`.

### Pruebas

Unitarias (rápidas, sin dependencias externas):

```bash
dotnet test tests/UserRegistration.Tests
```

De integración (levantan un PostgreSQL real con Testcontainers, corren los
mismos scripts de `database/` contra él, y prueban los repositorios reales y
la API completa vía `WebApplicationFactory`; requieren Docker corriendo):

```bash
dotnet test tests/UserRegistration.IntegrationTests
```

`dotnet test` a nivel de solución corre ambos proyectos.

## Interfaz web

`src/UserRegistration.Api/wwwroot` contiene una página simple (sin build ni
dependencias) que consume la API: formulario de registro con selects en
cascada (país → departamento → municipio), errores de validación mostrados
por campo, y un panel para consultar un usuario por número de documento. Se
sirve en `/` del mismo puerto de la API (no requiere CORS ni un contenedor aparte).

## Endpoints principales

| Método | Ruta                                                    | Descripción |
|--------|-----------------------------------------------------------|-------------|
| POST   | `/api/usuarios`                                            | Registra un usuario |
| GET    | `/api/usuarios/{numeroDocumento}`                          | Consulta un usuario |
| GET    | `/api/paises`                                              | Lista países |
| GET    | `/api/paises/{idPais}/departamentos`                       | Departamentos de un país |
| GET    | `/api/departamentos/{idDepartamento}/municipios`           | Municipios de un departamento |

### Ejemplo: registrar usuario

```bash
curl -X POST http://localhost:18080/api/usuarios \
  -H "Content-Type: application/json" \
  -d '{
        "numeroDocumento": "1020304050",
        "nombre": "Juan Camilo Espitia",
        "telefono": "+573001234567",
        "idPais": 1,
        "idDepartamento": 2,
        "idMunicipio": 3,
        "direccion": "Calle 123 # 45-67"
      }'
```

Usa `GET /api/paises`, `GET /api/paises/{idPais}/departamentos` y
`GET /api/departamentos/{idDepartamento}/municipios` para obtener ids válidos
según los datos semilla en `database/02_seed_data.sql`.

Si el municipio no pertenece al departamento/país indicado, la API responde
`400` con el detalle del campo que falló, en vez de solo comprobar que los
ids no sean nulos. Si el número de documento ya existe, responde `409`.
