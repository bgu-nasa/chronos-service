using Chronos.Data.Repositories.Auth;
using Chronos.MainApi.Auth.Contracts;
using Chronos.MainApi.Auth.Validation;
using Chronos.Shared.Exceptions;

namespace Chronos.MainApi.Auth.Services;

public class UserService(
    ILogger<UserService> logger,
    IUserRepository userRepository)
    : IUserService
{
    public async Task<UserResponse> GetUserAsync(Guid organizationId, Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        
        if (user is null)
        {
            throw new NotFoundException($"User with ID {userId} not found");
        }

        if (user.OrganizationId != organizationId)
        {
            throw new NotFoundException($"User with ID {userId} not found in organization {organizationId}");
        }

        return new UserResponse(
            user.Id.ToString(),
            user.Email,
            user.FirstName,
            user.LastName,
            user.AvatarUrl,
            user.Verified
        );
    }

    public async Task<IEnumerable<UserResponse>> GetUsersAsync(Guid organizationId)
    {
        var users = await userRepository.GetAllAsync();
        
        return users
            .Where(u => u.OrganizationId == organizationId)
            .Select(user => new UserResponse(
                user.Id.ToString(),
                user.Email,
                user.FirstName,
                user.LastName,
                user.AvatarUrl,
                user.Verified
            ))
            .ToList();
    }

    public async Task UpdateUserProfileAsync(Guid organizationId, Guid userId, UserUpdateRequest request)
    {
        AvatarUrlValidator.ValidateAvatarUrl(request.AvatarUrl);
        
        var user = await userRepository.GetByIdAsync(userId);
        
        if (user is null)
        {
            throw new NotFoundException($"User with ID {userId} not found");
        }

        if (user.OrganizationId != organizationId)
        {
            throw new NotFoundException($"User with ID {userId} not found in organization {organizationId}");
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.AvatarUrl = request.AvatarUrl;

        await userRepository.UpdateAsync(user);
        
        logger.LogInformation("Updated profile for user {UserId} in organization {OrganizationId}", userId, organizationId);
    }

    public async Task DeleteUserAsync(Guid organizationId, Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        
        if (user is null)
        {
            throw new NotFoundException($"User with ID {userId} not found");
        }

        if (user.OrganizationId != organizationId)
        {
            throw new NotFoundException($"User with ID {userId} not found in organization {organizationId}");
        }

        await userRepository.DeleteAsync(user);
        
        logger.LogInformation("Deleted user {UserId} from organization {OrganizationId}", userId, organizationId);
    }
}
