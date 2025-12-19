namespace TodoApi.Middleware;

using System.Net;
using TodoApi.Exceptions;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            TodoNotFoundException notFound =>
                (StatusCodes.Status404NotFound, notFound.Message, null as List<string>),

            ValidationException validation =>
                (StatusCodes.Status400BadRequest, "Validation failed", validation.Errors),

            ArgumentNullException =>
                (StatusCodes.Status400BadRequest, "Required parameter was null", null as List<string>),

            _ =>
                (StatusCodes.Status500InternalServerError, "An error occurred processing your request", null as List<string>)
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            statusCode,
            message,
            errors
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}