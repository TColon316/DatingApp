using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    // IHostEnvironment will be used to determine if we're in Production or Development mode
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // Determine the environment and then generate the appropriate error handling object
                var response = env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace)              // Developer Response - More Details
                    : new ApiException(context.Response.StatusCode, ex.Message, "Internal Server Error");   // Client Response - Less Details

                // I believe this sets it so that it will implement CamelCase logic when it converts to Json
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                // Converts the response to Json
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}