using Chronos.Data.Repositories.Auth;
using Chronos.Domain.Auth;
using Chronos.MainApi.Auth.Contracts;
using Chronos.Shared.Exceptions;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Chronos.MainApi.Auth.Services;

public class AuthService(
    IUserRepository userRepository,
    ITokenGenerator tokenGenerator)
    : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await userRepository.EmailExistsIgnoreFiltersAsync(request.AdminUser.Email))
        {
            throw new BadRequestException("User with this email already exists");
        }

        // TODO: Create organization
        var organizationId = Guid.NewGuid();

        var user = new User
        {
            FirstName = request.AdminUser.FirstName,
            LastName = request.AdminUser.LastName,
            Email = request.AdminUser.Email,
            PasswordHash = BCryptNet.HashPassword(request.AdminUser.Password),
            OrganizationId = organizationId,
            // TODO: Add roles and permissions, for now, the first user is an admin
        };

        await userRepository.AddAsync(user);

        var token = await tokenGenerator.GenerateTokenAsync(user);
        return new AuthResponse(token);
    }

    public async Task CreateUserAsync(string organizationId, CreateUserRequest request)
    {
        // TODO: Add authorization to check if the user can create a user in this organization

        if (await userRepository.GetByEmailAsync(request.Email) is not null)
        {
            throw new BadRequestException("User with this email already exists");
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = BCryptNet.HashPassword(request.Password),
            OrganizationId = Guid.Parse(organizationId)
        };

        await userRepository.AddAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByEmailIgnoreFiltersAsync(request.Email);

        if (user is null || !BCryptNet.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid credentials");
        }

        var token = await tokenGenerator.GenerateTokenAsync(user);
        return new AuthResponse(token);
    }

    public async Task<AuthResponse> RefreshTokenAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var token = await tokenGenerator.GenerateTokenAsync(user);
        return new AuthResponse(token);
    }

    public async Task VerifyTokenAsync(Guid userId)
    {
        if (!await userRepository.ExistsAsync(userId))
        {
            throw new UnauthorizedException("Invalid token");
        }
    }
}
