using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace PresentationLayer.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {

            try
            {
                await _next(context);
            }
            catch (BadHttpRequestException ex)
            {
                await HandleError(context, ex.Message, 400);
            }
            catch (Exception ex) {
                await HandleError(context, ex.Message, 500);
            }
        }

        private async Task HandleError(HttpContext context , string message , int statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        break;
                    }
                default:
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        break;
                    }
            }
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new BadResponseHandler
            {
                Message = message,
            }));
        }
    }
    internal class BadResponseHandler
    {
        public required string Message { get; set; }
    }
}
