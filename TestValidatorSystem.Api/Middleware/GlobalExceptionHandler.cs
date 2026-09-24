using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using TestValidatorSystem.Api.Models;

namespace TestValidatorSystem.Api.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken ct)
        {
            _logger.LogError(exception, "Необработанное исключение.");

            var response = new ResponseModel
            {
                IsError = 1,
                ErrorCode = "INTERNAL_ERROR",
                ErrorMessage = exception.Message
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json; charset=utf-8";

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            });

            await context.Response.WriteAsync(json, ct);
            return true;
        }
    }
}
