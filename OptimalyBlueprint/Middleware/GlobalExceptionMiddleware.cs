using System.Net;
using System.Text.Json;

namespace OptimalyBlueprint.Middleware;

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
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            success = false,
            message = "Vyskytla se neočekávaná chyba. Zkuste to prosím znovu.",
            details = exception.Message
        };

        // For AJAX requests, return JSON
        if (IsAjaxRequest(context.Request))
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
        else
        {
            // For regular requests, redirect to home with error message
            context.Response.StatusCode = (int)HttpStatusCode.Redirect;
            context.Response.Headers["Location"] = $"/?error={Uri.EscapeDataString(response.message)}";
        }
    }

    private static bool IsAjaxRequest(HttpRequest request)
    {
        return request.Headers.ContainsKey("X-Requested-With") && 
               request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}