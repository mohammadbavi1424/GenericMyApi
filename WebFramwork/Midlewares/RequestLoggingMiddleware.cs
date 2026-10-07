using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebFramework.Midlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Log.Information("Request: {Method} {Path}", context.Request.Method, context.Request.Path);

            await _next(context);

            Log.Information("Response: {StatusCode}", context.Response.StatusCode);
        }
    }

}
