namespace Attend.Web.Exceptions;

public class ApiException(string message, int statusCode = 500, string? title = null) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string? Title { get; } = title ?? "An error occurred";
}

public class ValidationApiException(string message) : ApiException(message, 400, "Validation Error");
