using Attend.Web.Models;
using Attend.Web.Services.Interfaces;
using System.Text.Json;

namespace Attend.Web.Services;

public class AttendanceService(IHttpClientFactory httpClientFactory, ILogger<AttendanceService> logger) : IAttendanceService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AttendApi");
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<AttendanceViewModel?> GetAttendanceByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/attendances/{id}");
        if (!response.IsSuccessStatusCode) return null;
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AttendanceViewModel>(json, _jsonOptions);
    }

    public async Task<PaginatedResponse<AttendanceViewModel>> GetEventAttendeesAsync(Guid eventId, PaginationRequest request)
    {
        try
        {
            var queryParams = $"?paginationRequest={{\"Page\":{request.Page},\"PageSize\":{request.PageSize}}}";
            var response = await _httpClient.GetAsync($"/events/{eventId}/attendees{queryParams}");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiPaginationResponse<AttendanceViewModel>>(json, _jsonOptions)!;

            return new PaginatedResponse<AttendanceViewModel>
            {
                Items = apiResponse.Items,
                TotalCount = apiResponse.TotalCount,
                Page = request.Page + 1,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting event attendees");
            throw new ApplicationException("Failed to get attendees.");
        }
    }

    public async Task<PaginatedResponse<AttendanceViewModel>> GetUserAttendancesAsync(Guid userId, PaginationRequest request)
    {
        try
        {
            var queryParams = $"?paginationRequest={{\"Page\":{request.Page},\"PageSize\":{request.PageSize}}}";
            var response = await _httpClient.GetAsync($"/users/{userId}/attendances{queryParams}");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiPaginationResponse<AttendanceViewModel>>(json, _jsonOptions)!;

            return new PaginatedResponse<AttendanceViewModel>
            {
                Items = apiResponse.Items,
                TotalCount = apiResponse.TotalCount,
                Page = request.Page + 1,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user attendances");
            throw new ApplicationException("Failed to get user attendances.");
        }
    }

    public async Task<Guid> RegisterAttendanceAsync(Guid userId, Guid eventId)
    {
        var json = JsonSerializer.Serialize(userId, _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"/events/{eventId}/register", content);
        response.EnsureSuccessStatusCode();
        
        var location = response.Headers.Location?.ToString();
        var idString = location?.Split('/').LastOrDefault();
        return Guid.Parse(idString!);
    }

    public async Task<bool> CheckInAsync(Guid attendanceId)
    {
        var response = await _httpClient.PostAsync($"/attendances/{attendanceId}/checkin", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CheckInAsync(Guid eventId, Guid userId)
    {
        var json = JsonSerializer.Serialize(new { userId }, _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"/events/{eventId}/checkin", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CheckInByQRCodeAsync(string qrCode)
    {
        var json = JsonSerializer.Serialize(qrCode, _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/checkin/qrcode", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CancelAttendanceAsync(Guid attendanceId)
    {
        var response = await _httpClient.DeleteAsync($"/attendances/{attendanceId}");
        return response.IsSuccessStatusCode;
    }
}
