using System.Net;
using System.Text.Json;
using Chronos.Shared.Exceptions;

namespace Chronos.MainApi.Shared.Middleware;

public class GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case BadRequestException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = response.StatusCode;
                errorResponse.Message = exception.Message;
                errorResponse.Type = "BadRequest";
                logger.LogWarning(exception, "Bad request: {Message}", exception.Message);
                break;

            case NotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.StatusCode = response.StatusCode;
                errorResponse.Message = exception.Message;
                errorResponse.Type = "NotFound";
                logger.LogWarning(exception, "Resource not found: {Message}", exception.Message);
                break;

            case UnauthorizedException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.StatusCode = response.StatusCode;
                errorResponse.Message = exception.Message;
                errorResponse.Type = "Unauthorized";
                logger.LogWarning(exception, "Unauthorized access: {Message}", exception.Message);
                break;

            case TokenMissingValueException:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.StatusCode = response.StatusCode;
                errorResponse.Message = "An internal server error occurred";
                errorResponse.Type = "InternalServerError";
                logger.LogCritical(exception, "Token missing required value: {Message}", exception.Message);
                break;

            case UnexpectedErrorException:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.StatusCode = response.StatusCode;
                errorResponse.Message = "An internal server error occurred";
                errorResponse.Type = "InternalServerError";
                logger.LogError(exception, "Unexpected error: {Message}", exception.Message);
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.StatusCode = response.StatusCode;
                errorResponse.Message = "An internal server error occurred";
                errorResponse.Type = "InternalServerError";
                logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }

    private class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
