using Attend.Web.Exceptions;
using System.Text.Json;

namespace Attend.Web.Extensions;

public static class HttpResponseExtensions
{
    public static async Task<T> ReadAsJsonOrThrowAsync<T>(this HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ApiException("Failed to deserialize response", 500);
        }

        // Try to parse ProblemDetails
        try
        {
            var problemDetails = JsonSerializer.Deserialize<ProblemDetailsResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var errorMessage = problemDetails?.Detail ?? content;
            var statusCode = (int)response.StatusCode;

            if (statusCode == 400)
                throw new ValidationApiException(errorMessage);

            throw new ApiException(errorMessage, statusCode, problemDetails?.Title);
        }
        catch (JsonException)
        {
            // If not ProblemDetails, throw with raw content
            throw new ApiException(content, (int)response.StatusCode);
        }
    }

    public static async Task EnsureSuccessOrThrowAsync(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        var content = await response.Content.ReadAsStringAsync();

        try
        {
            var problemDetails = JsonSerializer.Deserialize<ProblemDetailsResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var errorMessage = problemDetails?.Detail ?? content;
            var statusCode = (int)response.StatusCode;

            if (statusCode == 400)
                throw new ValidationApiException(errorMessage);

            throw new ApiException(errorMessage, statusCode, problemDetails?.Title);
        }
        catch (JsonException)
        {
            throw new ApiException(content, (int)response.StatusCode);
        }
    }

    private class ProblemDetailsResponse
    {
        public string? Title { get; set; }
        public string? Detail { get; set; }
        public int? Status { get; set; }
    }
}
