using System.Text.Json;
using BaseServerProject.Application.Common.Base;

namespace BaseServerProject.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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

        var response = exception switch
        {
            UnauthorizedAccessException => BaseResponse<object>.Failure("Unauthorized", 401),
            KeyNotFoundException => BaseResponse<object>.Failure("Resource not found", 404),
            ArgumentException => BaseResponse<object>.Failure(exception.Message, 400),
            _ => BaseResponse<object>.Failure("An internal error occurred", 500)
        };

        context.Response.StatusCode = response.StatusCode;
        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}