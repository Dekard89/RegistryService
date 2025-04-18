using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Spotify.Identity.Exeptions;
using ILogger = Serilog.ILogger;

namespace Spotify.Identity.Midlleware;

public class ValidationExeptionHandler(ILogger<ValidationException> _logger) : IExceptionHandler
{
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;
        int statusCode;

        switch (exception)
        {
            case FluentValidation.ValidationException validationException:
                _logger.LogWarning("Validation errors: {@Errors}", 
                    validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

                statusCode = validationException.Errors.Any(e => e.ErrorMessage.Contains("not found")) 
                    ? StatusCodes.Status404NotFound 
                    : StatusCodes.Status400BadRequest;

                problemDetails = new ValidationProblemDetails(
                    validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()))
                {
                    Title = "Validation failed",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Status = statusCode
                };
                break;

            case FailedCreatedException failedCreated:
                problemDetails = failedCreated.ProblemDetails;
                statusCode = failedCreated.ProblemDetails.Status ?? StatusCodes.Status400BadRequest;
                break;

            default:
                _logger.LogError(exception, "Unhandled exception occurred");
                problemDetails = new ProblemDetails
                {
                    Title = "Internal server error",
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                };
                statusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        
        problemDetails.Instance = httpContext.Request.Path;
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken)
            .ConfigureAwait(false);

        return true;
    }
}