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
        try
        {
            var queryParams = $"?paginationRequest={{\"SearchText\":\"{Uri.EscapeDataString(request.SearchText)}\",\"Page\":{request.Page},\"PageSize\":{request.PageSize}}}";
            var response = await _httpClient.GetAsync($"/events{queryParams}");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiPaginationResponse<EventViewModel>>(json, _jsonOptions)!;

            return new PaginatedResponse<EventViewModel>
            {
                Items = apiResponse.Items,
                TotalCount = apiResponse.TotalCount,
                Page = request.Page + 1,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting events");
            throw new ApplicationException("Failed to get events.");
        }
    }

    public async Task<EventViewModel?> GetEventByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/events/{id}");
        if (!response.IsSuccessStatusCode) return null;
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<EventViewModel>(json, _jsonOptions);
    }

    public async Task<Guid> CreateEventAsync(EventCreateViewModel model)
    {
        var json = JsonSerializer.Serialize(model, _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/events", content);
        response.EnsureSuccessStatusCode();
        
        var location = response.Headers.Location?.ToString();
        var idString = location?.Split('/').LastOrDefault();
        return Guid.Parse(idString!);
    }

    public async Task<bool> UpdateEventAsync(Guid id, EventUpdateViewModel model)
    {
        var json = JsonSerializer.Serialize(model, _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"/events/{id}", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteEventAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/events/{id}");
        return response.IsSuccessStatusCode;
    }
}
