using Chronos.MainApi.Auth.Contracts;

namespace Chronos.MainApi.Auth.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<CreateUserResponse> CreateUserAsync(string organizationId, CreateUserRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(Guid userId);
    Task VerifyTokenAsync(Guid userId);
    Task UpdatePasswordAsync(Guid userId, UserPasswordUpdateRequest request);
}