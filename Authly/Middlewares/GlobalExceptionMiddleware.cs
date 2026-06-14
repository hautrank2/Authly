using Authly.Models;
using System.Net;
using System.Text.Json;

namespace Authly.Middlewares
{
    public class GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment env)
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, errors) = ex switch
            {
                ArgumentException or InvalidOperationException =>
                    (HttpStatusCode.BadRequest, new List<string> { ex.Message }),

                KeyNotFoundException =>
                    (HttpStatusCode.NotFound, new List<string> { ex.Message }),

                UnauthorizedAccessException =>
                    (HttpStatusCode.Unauthorized, new List<string> { "Unauthorized" }),

                _ =>
                    (HttpStatusCode.InternalServerError, new List<string> { "An unexpected error occurred" })
            };

            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse.Fail(errors, "Error");

            if (env.IsDevelopment())
                response.Detail = $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";

            var json = JsonSerializer.Serialize(response, _jsonOptions);
            await context.Response.WriteAsync(json);
        }
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
