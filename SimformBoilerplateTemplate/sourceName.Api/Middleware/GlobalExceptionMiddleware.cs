using FluentValidation;
using sourceName.DTO;
using System.Net;
using System.Text.Json;

namespace sourceName.Api.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = exception switch
        {
            ValidationException validationEx => new BaseResponse<object>
            {
                IsSuccess = false,
                Message = "Validation failed",
                Errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList(),
                Timestamp = DateTime.UtcNow
            },
            ArgumentException argEx => new BaseResponse<object>
            {
                IsSuccess = false,
                Message = argEx.Message,
                Timestamp = DateTime.UtcNow
            },
            UnauthorizedAccessException => new BaseResponse<object>
            {
                IsSuccess = false,
                Message = "Unauthorized access",
                Timestamp = DateTime.UtcNow
            },
            KeyNotFoundException => new BaseResponse<object>
            {
                IsSuccess = false,
                Message = "Resource not found",
                Timestamp = DateTime.UtcNow
            },
            _ => new BaseResponse<object>
            {
                IsSuccess = false,
                Message = "An internal server error occurred",
                Timestamp = DateTime.UtcNow
            }
        };

        context.Response.StatusCode = exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
