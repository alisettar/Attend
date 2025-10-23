using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Attend.Api.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An error occurred");

        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred",
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        if (exception is ValidationException validationException)
        {
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Validation error";

            // Extract clean error messages
            if (validationException.Errors?.Any() == true)
            {
                var errorMessages = validationException.Errors
                    .Select(e => e.ErrorMessage)
                    .Where(msg => !string.IsNullOrWhiteSpace(msg))
                    .ToList();

                // Use first error message as detail (user-friendly)
                problemDetails.Detail = errorMessages.FirstOrDefault() ?? validationException.Message;

                // Add all errors to extensions for API consumers
                var errors = validationException.Errors
                    .Select(e => new
                    {
                        Field = e.PropertyName.Replace("Request.", "").Replace(".", ""),
                        Message = e.ErrorMessage
                    });
                problemDetails.Extensions.Add("errors", errors);
            }
            else
            {
                problemDetails.Detail = validationException.Message;
            }
        }

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

        return true;
    }
}
