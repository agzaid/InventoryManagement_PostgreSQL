using Application.Interfaces.Contracts.Localization;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace Infrastructure
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                var localizer = context.RequestServices.GetRequiredService<IAppLocalizer>();

                // Log the exception immediately
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

                if (IsAjaxRequest(context))
                {
                    // Handle AJAX/API requests with JSON
                    await HandleAjaxExceptionAsync(context, ex, localizer);
                }
                else
                {
                    // Handle Standard Page requests with a Redirect to an Error View
                    HandleViewException(context, ex);
                }
            }
        }

        private static bool IsAjaxRequest(HttpContext context)
        {
            // Check for common AJAX header
            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return true;

            // Check if the client explicitly prefers JSON over HTML
            var acceptHeader = context.Request.Headers["Accept"].ToString();
            if (acceptHeader.Contains("application/json") && !acceptHeader.Contains("text/html"))
                return true;

            return false;
        }

        private void HandleViewException(HttpContext context, Exception exception)
        {
            // 1. Determine the message based on the exception type
            string message = exception switch
            {
                BadRequestException => exception.Message,
                NotFoundException => exception.Message,
                _ => "An unexpected server error occurred."
            };

            // 2. Log the error (already done in InvokeAsync)

            // 3. Redirect to Home and pass the error message via Query String
            // We URI Encode the message to ensure it handles spaces/special characters
            var encodedMessage = Uri.EscapeDataString(message);
            context.Response.Redirect($"/Home/Index?errorMessage={encodedMessage}");
        }

        private async Task HandleAjaxExceptionAsync(HttpContext context, Exception exception, IAppLocalizer localizer)
        {
            int statusCode;
            string title;
            string detail = exception switch
            {
                BadRequestException => exception.Message,
                NotFoundException => exception.Message,
                _ => localizer["UnexpectedErrorDetail"]
            };

            switch (exception)
            {
                case BadRequestException:
                    statusCode = StatusCodes.Status400BadRequest;
                    title = localizer["BadRequest"];
                    break;

                case NotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    title = localizer["NotFound"];
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    title = localizer["ServerError"];
                    break;
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            };

            context.Response.Clear(); // Ensure nothing else was written
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}