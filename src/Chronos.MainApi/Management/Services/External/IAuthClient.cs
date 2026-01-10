using Chronos.MainApi.Auth.Contracts;

namespace Chronos.MainApi.Management.Services.External;

public interface IAuthClient
{
    Task<UserResponse> GetUserAsync(Guid organizationId, Guid userId);
    Task<IEnumerable<UserResponse>> GetUsersAsync(Guid organizationId);
}