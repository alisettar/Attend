using Attend.Web.Models;

namespace Attend.Web.Services.Interfaces;

public interface IUserService
{
    Task<PaginatedResponse<UserViewModel>> GetUsersAsync(PaginationRequest request);
    Task<UserViewModel?> GetUserByIdAsync(Guid id);
    Task<UserViewModel?> GetUserByQRCodeAsync(string qrCode);
    Task<Guid> CreateUserAsync(UserCreateViewModel model);
    Task<bool> UpdateUserAsync(Guid id, UserUpdateViewModel model);
    Task<bool> DeleteUserAsync(Guid id);
}
