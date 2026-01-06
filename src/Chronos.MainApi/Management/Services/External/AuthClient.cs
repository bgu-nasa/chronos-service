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
}