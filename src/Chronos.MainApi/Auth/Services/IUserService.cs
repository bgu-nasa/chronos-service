using Chronos.MainApi.Auth.Contracts;

namespace Chronos.MainApi.Auth.Services;

public interface IUserService
{
    Task<UserResponse> GetUserAsync(Guid organizationId, Guid userId);
    Task<IEnumerable<UserResponse>> GetUsersAsync(Guid organizationId);
    Task UpdateUserProfileAsync(Guid organizationId, Guid userId, UserUpdateRequest request);
    Task DeleteUserAsync(Guid organizationId, Guid userId);
}