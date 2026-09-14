using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using UserRegistration.Domain.Exceptions;

namespace UserRegistration.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleAsync(context, exception);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (statusCode, problemDetails) = Map(context, exception);

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Error no controlado procesando {Method} {Path}",
                context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "Solicitud rechazada ({StatusCode}) en {Method} {Path}",
                (int)statusCode, context.Request.Method, context.Request.Path);
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, problemDetails.GetType(), JsonOptions));
    }

    private static (HttpStatusCode StatusCode, ProblemDetails Problem) Map(HttpContext context, Exception exception)
    {
        switch (exception)
        {
            case ValidationAppException validationException:
            {
                var problem = new ValidationProblemDetails(new Dictionary<string, string[]>(validationException.Errors))
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Uno o más campos son inválidos.",
                    Instance = context.Request.Path
                };
                return (HttpStatusCode.BadRequest, problem);
            }

            case NotFoundException notFoundException:
            {
                var problem = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Title = "Recurso no encontrado.",
                    Detail = notFoundException.Message,
                    Instance = context.Request.Path
                };
                return (HttpStatusCode.NotFound, problem);
            }

            case ConflictException conflictException:
            {
                var problem = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Conflict,
                    Title = "Conflicto al persistir la información.",
                    Detail = conflictException.Message,
                    Instance = context.Request.Path
                };
                return (HttpStatusCode.Conflict, problem);
            }

            default:
            {
                var problem = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Ocurrió un error inesperado al procesar la solicitud.",
                    Instance = context.Request.Path
                };
                return (HttpStatusCode.InternalServerError, problem);
            }
        }
    }
}
