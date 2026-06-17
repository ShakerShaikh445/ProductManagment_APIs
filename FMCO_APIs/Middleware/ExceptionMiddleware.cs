using System.Text.Json;

namespace ProductManagment_APIs.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var result = new
                {
                    Success = false,
                    Message = ex.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(result));
            }
        }
    }
}