using Attend.Web.Models;
using Attend.Web.Services.Interfaces;
using System.Text.Json;

namespace Attend.Web.Services;

public class UserService(IHttpClientFactory httpClientFactory, ILogger<UserService> logger) : IUserService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AttendApi");
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<PaginatedResponse<UserViewModel>> GetUsersAsync(PaginationRequest request)
    {
        try
        {
            var queryParams = $"?paginationRequest={{\"SearchText\":\"{Uri.EscapeDataString(request.SearchText)}\",\"Page\":{request.Page},\"PageSize\":{request.PageSize}}}";
            var response = await _httpClient.GetAsync($"/users{queryParams}");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiPaginationResponse<UserViewModel>>(json, _jsonOptions)!;

            return new PaginatedResponse<UserViewModel>
            {
                Items = apiResponse.Items,
                TotalCount = apiResponse.TotalCount,
                Page = request.Page + 1,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting users");
            throw new ApplicationException("Failed to get users.");
        }
    }

    public async Task<UserViewModel?> GetUserByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/users/{id}");
        if (!response.IsSuccessStatusCode) return null;
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserViewModel>(json, _jsonOptions);
    }

    public async Task<UserViewModel?> GetUserByQRCodeAsync(string qrCode)
    {
        var response = await _httpClient.GetAsync($"/users/qrcode/{Uri.EscapeDataString(qrCode)}");
        if (!response.IsSuccessStatusCode) return null;
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserViewModel>(json, _jsonOptions);
    }

    public async Task<Guid> CreateUserAsync(UserCreateViewModel model)
    {
        var json = JsonSerializer.Serialize(model, _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/users", content);
        response.EnsureSuccessStatusCode();
        
        var location = response.Headers.Location?.ToString();
        var idString = location?.Split('/').LastOrDefault();
        return Guid.Parse(idString!);
    }

    public async Task<bool> UpdateUserAsync(Guid id, UserUpdateViewModel model)
    {
        var json = JsonSerializer.Serialize(model, _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"/users/{id}", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/users/{id}");
        return response.IsSuccessStatusCode;
    }
}
