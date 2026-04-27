using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;


namespace VehicleIMS_backend.Application.Middlewares
{
    public class ExceptionHandlerMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext httpContext)
        {
            try
            { 
                await _next(httpContext);
            }
            catch (UnauthorizedAccessException ex)
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    message = ex.Message,
                    statusCode = 401
                }));
            }
            catch (Exception ex)
            {
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
