using Chronos.MainApi.Auth.Contracts;
using Chronos.MainApi.Auth.Services;

namespace Chronos.MainApi.Management.Services.External;

public class AuthClient(ILogger<AuthClient> logger,
    IUserService userService) : IAuthClient
{
    public async Task<UserResponse> GetUserAsync(Guid organizationId, Guid userId)
    {
        logger.LogInformation("Management accessing auth for user fetch with id: {userId}", userId);

        return await userService.GetUserAsync(organizationId, userId);
    }

    public async Task<IEnumerable<UserResponse>> GetUsersAsync(Guid organizationId)
    {
        logger.LogInformation("Management accessing auth for users fetch for organization with id: {organizationId}", organizationId);

        return await userService.GetUsersAsync(organizationId);
    }
}