using Chronos.Domain.Auth;

namespace Chronos.MainApi.Auth.Services;

public interface ITokenGenerator
{
    string GenerateToken(User user);
}