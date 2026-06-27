using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System;
using System.Threading.Tasks;
using TPI_2026.Application.Exceptions;
using TPI_2026.Domain.Exceptions;


namespace TPI_2026.Presentation.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            if (exception is NotFoundException or ForbiddenException or ValidationException or DomainException)
            {
                context.Response.StatusCode = exception switch
                {
                    NotFoundException => (int)HttpStatusCode.NotFound,
                    ForbiddenException => (int)HttpStatusCode.Forbidden,
                    ValidationException => (int)HttpStatusCode.BadRequest,
                    DomainException => (int)HttpStatusCode.BadRequest,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = exception.Message
                };

                return context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var result = JsonSerializer.Serialize(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace
                });

                return context.Response.WriteAsync(result);
            }
        }
    }
}
