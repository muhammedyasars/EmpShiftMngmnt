using Domain;

namespace EmpShifMngmnt.Middleware;
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        var (statusCode, code, message) = exception switch
        {
            ValidationException validationException => (StatusCodes.Status400BadRequest, validationException.Code, validationException.Message),
            NotFoundException notFoundException => (StatusCodes.Status404NotFound, notFoundException.Code, notFoundException.Message),
            BusinessRuleException businessRuleException => (StatusCodes.Status400BadRequest, businessRuleException.Code, businessRuleException.Message),
            _ => (StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "An internal server error occurred.")
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = message,
            code = code
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}