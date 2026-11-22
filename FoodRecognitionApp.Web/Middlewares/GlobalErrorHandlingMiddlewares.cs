using FoodRecognitionApp.Domain.Exceptions.BadRequest;
using FoodRecognitionApp.Domain.Exceptions.NotFound;
using FoodRecognitionApp.Domain.Exceptions.UnAuthorized;
using FoodRecognitionApp.Shared.ErrorModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FoodRecognitionApp.Web.Middlewares
{
    internal class GlobalErrorHandlingMiddlewares(RequestDelegate _next, ILogger<GlobalErrorHandlingMiddlewares> _logger, IHostEnvironment _environment)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
                if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Response.HasStarted && string.IsNullOrEmpty(context.Response.ContentType))
                    await HandleNotFoundEndpointAsync(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, cannot write error response.");
                    throw;
                }
                await HandleUnhandledExceptionAsync(context, ex);
            }
        }
        private static async Task HandleNotFoundEndpointAsync(HttpContext context)
        {
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Endpoint Not Found",
                Detail = $"The requested endpoint '{context.Request.Path}' was not found on this server.",
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
            await context.Response.WriteAsJsonAsync(problem);
        }
        private async Task HandleUnhandledExceptionAsync(HttpContext context, Exception ex)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            _logger.LogError(ex, "Unhandled server error at {Path}. TraceId: {TraceId}", context.Request.Path, traceId);

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = _environment.IsDevelopment()
                    ? ex.Message
                    : "An unexpected error occurred. Please contact support with the trace ID.",
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = traceId;
            if (_environment.IsDevelopment())
            {
                problem.Extensions["stackTrace"] = ex.StackTrace;
            }
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
