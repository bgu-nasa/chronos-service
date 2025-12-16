using Chronos.Domain.Auth;

namespace Chronos.MainApi.Auth.Services;

public interface ITokenGenerator
{
    Task<string> GenerateTokenAsync(User user);
}