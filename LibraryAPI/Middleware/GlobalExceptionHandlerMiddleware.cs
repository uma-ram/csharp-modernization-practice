namespace LibraryAPI.Middleware;
using LibraryAPI.Exceptions;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, 
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
            _logger.LogError(ex, "An unhandled exception occurred.");            
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var (statusCode, message, errors) = exception switch
        {
            BookNotFoundException notFound =>
            (StatusCodes.Status404NotFound, notFound.Message, null as List<string>),

            MemberNotFoundException notFound =>
            (StatusCodes.Status404NotFound, notFound.Message, null as List<string>),

            BookNotAvailableException notAvailable =>
            (StatusCodes.Status400BadRequest, notAvailable.Message, null as List<string>),

            ValidationException validation =>
            (StatusCodes.Status400BadRequest, "Validation Failed", validation.Errors),

            _ =>
            (StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.", null as List<string>)
        };

        context.Response.StatusCode = statusCode;
        var response = new
        {
            StatusCode = statusCode,
            Message = message,
            Errors = errors
        };
        await context.Response.WriteAsJsonAsync(response);
    }
}
