using Attend.Web.Extensions;
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
        try
        {
            var response = await _httpClient.GetAsync($"/attendances/{id}");
            return await response.ReadAsJsonOrThrowAsync<AttendanceViewModel>();
        }
        catch (Attend.Web.Exceptions.ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    public async Task<PaginatedResponse<AttendanceViewModel>> GetEventAttendeesAsync(Guid eventId, PaginationRequest request)
    {
        var queryParams = $"?paginationRequest={{\"Page\":{request.Page},\"PageSize\":{request.PageSize}}}";
        var response = await _httpClient.GetAsync($"/events/{eventId}/attendees{queryParams}");
        var apiResponse = await response.ReadAsJsonOrThrowAsync<ApiPaginationResponse<AttendanceViewModel>>();

        return new PaginatedResponse<AttendanceViewModel>
        {
            Items = apiResponse.Items,
            TotalCount = apiResponse.TotalCount,
            Page = request.Page + 1,
            PageSize = request.PageSize
        };
    }

    public async Task<PaginatedResponse<AttendanceViewModel>> GetUserAttendancesAsync(Guid userId, PaginationRequest request)
    {
        var queryParams = $"?paginationRequest={{\"Page\":{request.Page},\"PageSize\":{request.PageSize}}}";
        var response = await _httpClient.GetAsync($"/users/{userId}/attendances{queryParams}");
        var apiResponse = await response.ReadAsJsonOrThrowAsync<ApiPaginationResponse<AttendanceViewModel>>();

        return new PaginatedResponse<AttendanceViewModel>
        {
            Items = apiResponse.Items,
            TotalCount = apiResponse.TotalCount,
            Page = request.Page + 1,
            PageSize = request.PageSize
        };
    }

    public async Task<Guid> RegisterAttendanceAsync(Guid userId, Guid eventId)
    {
        var json = JsonSerializer.Serialize(userId, _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"/events/{eventId}/register", content);
        await response.EnsureSuccessOrThrowAsync();

        var location = response.Headers.Location?.ToString();
        var idString = location?.Split('/').LastOrDefault();
        return Guid.Parse(idString!);
    }

    public async Task<bool> CheckInAsync(Guid attendanceId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/attendances/{attendanceId}/checkin", null);
            await response.EnsureSuccessOrThrowAsync();
            return true;
        }
        catch (Attend.Web.Exceptions.ApiException ex) when (ex.StatusCode == 404)
        {
            return false;
        }
    }

    public async Task<bool> CheckInAsync(Guid eventId, Guid userId)
    {
        try
        {
            var json = JsonSerializer.Serialize(new { userId }, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"/events/{eventId}/checkin", content);
            await response.EnsureSuccessOrThrowAsync();
            return true;
        }
        catch (Attend.Web.Exceptions.ApiException ex) when (ex.StatusCode == 404)
        {
            return false;
        }
    }

    public async Task<CheckInResultViewModel> CheckInByQRCodeAsync(string qrCode, Guid eventId)
    {
        var request = new { qrCode, eventId };
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/checkin/qrcode", content);

        return await response.ReadAsJsonOrThrowAsync<CheckInResultViewModel>();
    }

    public async Task<bool> DeleteAttendanceAsync(Guid attendanceId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/attendances/{attendanceId}");
            await response.EnsureSuccessOrThrowAsync();
            return true;
        }
        catch (Exceptions.ApiException ex) when (ex.StatusCode == 404)
        {
            return false;
        }
    }
}
