using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using VehicleIMS_backend.Application.Exceptions;


namespace VehicleIMS_backend.Application.Middlewares
{
    public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger = logger;

        public async Task Invoke(HttpContext httpContext)
        {
            try
            { 
                await _next(httpContext);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access occurred");
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    message = ex.Message,
                    statusCode = 401
                }));
            }
            catch(NotFoundException ex)
            {
                _logger.LogError(ex, "Not Found Exception occurred");
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = 404;
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    message = ex.Message,
                    statusCode = 404
                }));
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Bad Request Exception occurred");
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    message = ex.Message,
                    statusCode = 400
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred");
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    message = ex.Message,
                    statusCode = 500
                }));
            }
        }
    }
}
