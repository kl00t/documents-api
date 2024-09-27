using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Documents.Core.Exceptions;
using Documents.Service.Exceptions;

namespace Documents.Api;

public class CustomExceptionFilter : IExceptionFilter
{
    private readonly ILogger<CustomExceptionFilter> _logger;

    public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        // Log the exception details for debugging
        _logger.LogError(context.Exception, "An error occurred: {Message}", context.Exception.Message);

        // Prepare the ProblemDetails response
        var problemDetails = new ProblemDetails
        {
            Type = "https://example.com/error", // Can point to a documentation page explaining error types
            Title = "An error occurred while processing your request.",
            Detail = context.Exception.Message, // Include the exception message
            Status = 500,  // Default to 500 Internal Server Error
            Instance = context.HttpContext.Request.Path  // Include the request path
        };

        if (context.Exception is S3ClientException)
        {
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Title = "S3 Client Error";
        }
        else if (context.Exception is ServiceException)
        {
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Title = "Service Error";
        }
        else
        {
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Title = "Internal Server Error";
        }

        // Set the result to return the ProblemDetails object with the correct status code
        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };

        context.ExceptionHandled = true;
    }
}
