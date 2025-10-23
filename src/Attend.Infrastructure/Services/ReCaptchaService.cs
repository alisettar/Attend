using Attend.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Attend.Infrastructure.Services;

public class ReCaptchaService(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<ReCaptchaService> logger) : IReCaptchaService
{
    private readonly string _secretKey = configuration["GoogleReCaptcha:SecretKey"]
            ?? throw new InvalidOperationException("ReCaptcha SecretKey is not configured");
    private const string VerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

    public async Task<bool> VerifyTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            logger.LogWarning("ReCaptcha token is empty");
            return false;
        }

        try
        {
            logger.LogInformation("Verifying reCAPTCHA token (first 20 chars): {Token}", token.Substring(0, Math.Min(20, token.Length)));
            logger.LogInformation("Using secret key (first 10 chars): {SecretKey}", _secretKey.Substring(0, Math.Min(10, _secretKey.Length)));

            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", _secretKey),
                new KeyValuePair<string, string>("response", token)
            });

            var response = await httpClient.PostAsync(VerifyUrl, requestContent, cancellationToken);
            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

            logger.LogInformation("ReCaptcha API response: {Response}", jsonString);

            var result = JsonSerializer.Deserialize<ReCaptchaResponse>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result?.Success == true)
            {
                logger.LogInformation("ReCaptcha verification successful. Score: {Score}, Action: {Action}", result.Score, result.Action);

                // For v3, check score (0.0 - 1.0, higher is better)
                // 0.5 is a reasonable threshold
                if (result.Score >= 0.5)
                {
                    return true;
                }
                else
                {
                    logger.LogWarning("ReCaptcha score too low: {Score}", result.Score);
                    return false;
                }
            }

            var errors = result?.ErrorCodes != null && result.ErrorCodes.Length > 0
                ? string.Join(", ", result.ErrorCodes)
                : "No specific error codes";

            logger.LogWarning("ReCaptcha verification failed. Success: {Success}, Errors: {Errors}",
                result?.Success, errors);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error verifying ReCaptcha token");
            return false;
        }
    }

    private class ReCaptchaResponse
    {
        public bool Success { get; set; }
        public double Score { get; set; }
        public string Action { get; set; } = string.Empty;

        [JsonPropertyName("challenge_ts")]
        public DateTime ChallengeTs { get; set; }

        public string Hostname { get; set; } = string.Empty;

        [JsonPropertyName("error-codes")]
        public string[] ErrorCodes { get; set; } = Array.Empty<string>();
    }
}
