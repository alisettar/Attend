using Attend.Web.Extensions;
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
        var queryParams = $"?paginationRequest={{\"SearchText\":\"{Uri.EscapeDataString(request.SearchText)}\",\"Page\":{request.Page},\"PageSize\":{request.PageSize}}}";
        var response = await _httpClient.GetAsync($"/users{queryParams}");
        var apiResponse = await response.ReadAsJsonOrThrowAsync<ApiPaginationResponse<UserViewModel>>();

        return new PaginatedResponse<UserViewModel>
        {
            Items = apiResponse.Items,
            TotalCount = apiResponse.TotalCount,
            Page = request.Page + 1,
            PageSize = request.PageSize
        };
    }

    public async Task<UserViewModel?> GetUserByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/users/{id}");
            return await response.ReadAsJsonOrThrowAsync<UserViewModel>();
        }
        catch (Attend.Web.Exceptions.ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    public async Task<UserViewModel?> GetUserByQRCodeAsync(string qrCode)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/users/qrcode/{Uri.EscapeDataString(qrCode)}");
            return await response.ReadAsJsonOrThrowAsync<UserViewModel>();
        }
        catch (Attend.Web.Exceptions.ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    public async Task<Guid> CreateUserAsync(UserCreateViewModel model)
    {
        var json = JsonSerializer.Serialize(model, _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/users", content);
        await response.EnsureSuccessOrThrowAsync();

        var location = response.Headers.Location?.ToString();
        var idString = location?.Split('/').LastOrDefault();
        return Guid.Parse(idString!);
    }

    public async Task<bool> UpdateUserAsync(Guid id, UserUpdateViewModel model)
    {
        try
        {
            var json = JsonSerializer.Serialize(model, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/users/{id}", content);
            await response.EnsureSuccessOrThrowAsync();
            return true;
        }
        catch (Attend.Web.Exceptions.ApiException ex) when (ex.StatusCode == 404)
        {
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/users/{id}");
            await response.EnsureSuccessOrThrowAsync();
            return true;
        }
        catch (Attend.Web.Exceptions.ApiException ex) when (ex.StatusCode == 404)
        {
            return false;
        }
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
}
