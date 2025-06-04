using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TicketingSystem.Models.Common;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string? Title { get; set; } // A brief, human-readable summary of the error
    public string? Message { get; set; } // More detailed, actionable message for the client
    public string? TraceId { get; set; } // Correlation ID for tracing requests across systems

    // Dictionary to hold validation errors, mapping field names to a list of errors
    // Nullable if there are no validation errors
    public Dictionary<string, List<string>>? ValidationErrors { get; set; }

    private ErrorResponse(int statusCode, string title, string message, string traceId)
    {
        StatusCode = statusCode;
        Title = title;
        Message = message;
        TraceId = traceId;
    }

    /// <summary>
    /// Creates an ErrorResponse specifically for Bad Request (400) due to model state validation errors.
    /// </summary>
    /// <param name="httpContext">The HttpContext from the controller.</param>
    /// <param name="modelState">The ModelStateDictionary containing validation errors.</param>
    /// <param name="customMessage">An optional custom message for the client.</param>
    /// <returns>A new ErrorResponse instance.</returns>
    public static ErrorResponse BadRequest(
        HttpContext httpContext,
        ModelStateDictionary modelState,
        string customMessage = "One or more validation errors occurred."
    )
    {
        var errorResponse = new ErrorResponse(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Bad Request",
            message: customMessage,
            traceId: httpContext?.TraceIdentifier ?? Activity.Current?.Id // Use HttpContext TraceIdentifier or Activity ID
        );

        errorResponse.ValidationErrors = new Dictionary<string, List<string>>();
        foreach (var key in modelState.Keys)
        {
            var errors = modelState[key]?.Errors;
            if (errors is not null && errors.Any())
            {
                errorResponse.ValidationErrors.Add(
                    key,
                    errors.Select(x => x.ErrorMessage).ToList()
                );
            }
        }
        return errorResponse;
    }

    public static ErrorResponse OnlyMessage(
        HttpContext? httpContext,
        ModelStateDictionary? modelState,
        string title,
        string customMessage
    )
    {
        var errorResponse = new ErrorResponse(
            statusCode: StatusCodes.Status400BadRequest,
            title: title,
            message: customMessage,
            traceId: httpContext?.TraceIdentifier ?? Activity.Current?.Id // Use HttpContext TraceIdentifier or Activity ID
        );

        errorResponse.ValidationErrors = new Dictionary<string, List<string>>();
        if (modelState is not null)
            foreach (var key in modelState.Keys)
            {
                var errors = modelState[key]?.Errors;
                if (errors is not null && errors.Any())
                {
                    errorResponse.ValidationErrors.Add(
                        key,
                        errors.Select(x => x.ErrorMessage).ToList()
                    );
                }
            }

        return errorResponse;
    }

    /// <summary>
    /// Creates a generic ErrorResponse for other types of errors (e.g., 404, 500, 403).
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="title">A brief summary of the error (e.g., "Not Found", "Internal Server Error").</param>
    /// <param name="message">A detailed message for the client.</param>
    /// <param name="httpContext">The HttpContext from the controller (optional, for TraceId).</param>
    /// <returns>A new ErrorResponse instance.</returns>
    public static ErrorResponse Create(
        int statusCode,
        string title,
        string message,
        HttpContext? httpContext
    )
    {
        return new ErrorResponse(
            statusCode: statusCode,
            title: title,
            message: message,
            traceId: httpContext?.TraceIdentifier ?? Activity.Current?.Id
        );
    }
}
