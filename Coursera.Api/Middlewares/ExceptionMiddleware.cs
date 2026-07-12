using System.Net;
using System.Text.Json;
using Coursera.Application.Common.Exceptions;

namespace Coursera.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";

                context.Response.StatusCode = ex switch
                {
                    ValidationException  => (int)HttpStatusCode.BadRequest,
                    UnauthorizedException => (int)HttpStatusCode.Unauthorized,
                    NotFoundException    => (int)HttpStatusCode.NotFound,
                    _                    => (int)HttpStatusCode.InternalServerError
                };

                // For ValidationException surface the structured Errors map so
                // the client knows exactly which fields failed and why.
                object response = ex is ValidationException ve
                    ? new { message = ve.Message, errors = ve.Errors }
                    : new { message = ex.Message };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
