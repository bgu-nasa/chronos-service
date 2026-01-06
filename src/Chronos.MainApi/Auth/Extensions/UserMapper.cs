using Chronos.Domain.Auth;
using Chronos.MainApi.Auth.Contracts;

namespace Chronos.MainApi.Auth.Extensions;

public static class UserMapper
{
    public static UserResponse ToUserResponse(this User user)
    {
        return new UserResponse(
            user.Id.ToString(),
            user.Email,
            user.FirstName,
            user.LastName,
            user.AvatarUrl,
            user.Verified
        );
    }
}
