using System.Net;
using System.Text.Json;
using Northwind.DTO;

namespace Northwind.Exceptions;

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
        catch (ApplicationException ex)
        {
            var responseContent = new HttpErrorResponse
            {
                ErrorCode = ex.ErrorCode,
                Message = ex.Message
            };
            await HandleExceptionAsync(context, ex, responseContent);
        }
        catch (Exception ex)
        {
            var responseContent = new HttpErrorResponse
            {
                ErrorCode = "InternalServerError",
                Message = ex.Message
            };
            await HandleExceptionAsync(context, ex, responseContent);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex, HttpErrorResponse response)
    {

        var statusCode = ex switch
        {
            NotFoundException => StatusCodes.Status404NotFound,

            ValidationException => StatusCodes.Status400BadRequest,

            _ => StatusCodes.Status500InternalServerError
        };
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";


        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}