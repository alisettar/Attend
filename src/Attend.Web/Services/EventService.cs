using Attend.Web.Extensions;
using Attend.Web.Models;
using Attend.Web.Services.Interfaces;
using System.Text.Json;

namespace Attend.Web.Services;

public class EventService(IHttpClientFactory httpClientFactory, ILogger<EventService> logger) : IEventService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AttendApi");
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<PaginatedResponse<EventViewModel>> GetEventsAsync(PaginationRequest request)
    {
        var queryParams = $"?paginationRequest={{\"SearchText\":\"{Uri.EscapeDataString(request.SearchText)}\",\"Page\":{request.Page},\"PageSize\":{request.PageSize}}}";
        var response = await _httpClient.GetAsync($"/events{queryParams}");
        var apiResponse = await response.ReadAsJsonOrThrowAsync<ApiPaginationResponse<EventViewModel>>();

        return new PaginatedResponse<EventViewModel>
        {
            Items = apiResponse.Items,
            TotalCount = apiResponse.TotalCount,
            Page = request.Page + 1,
            PageSize = request.PageSize
        };
    }

    public async Task<EventViewModel?> GetEventByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/events/{id}");
            return await response.ReadAsJsonOrThrowAsync<EventViewModel>();
        }
        catch (Attend.Web.Exceptions.ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    public async Task<EventStatisticsViewModel?> GetEventStatisticsAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/events/{id}/statistics");
            return await response.ReadAsJsonOrThrowAsync<EventStatisticsViewModel>();
        }
        catch (Attend.Web.Exceptions.ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    public async Task<Guid> CreateEventAsync(EventCreateViewModel model)
    {
        var json = JsonSerializer.Serialize(model, _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/events", content);
        await response.EnsureSuccessOrThrowAsync();

        var location = response.Headers.Location?.ToString();
        var idString = location?.Split('/').LastOrDefault();
        return Guid.Parse(idString!);
    }

    public async Task<bool> UpdateEventAsync(Guid id, EventUpdateViewModel model)
    {
        try
        {
            var json = JsonSerializer.Serialize(model, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/events/{id}", content);
            await response.EnsureSuccessOrThrowAsync();
            return true;
        }
        catch (Attend.Web.Exceptions.ApiException ex) when (ex.StatusCode == 404)
        {
            return false;
        }
    }

    public async Task<bool> DeleteEventAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/events/{id}");
            await response.EnsureSuccessOrThrowAsync();
            return true;
        }
        catch (Attend.Web.Exceptions.ApiException ex) when (ex.StatusCode == 404)
        {
            return false;
        }
    }
}
