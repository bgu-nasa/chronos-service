namespace Chronos.MainApi.Auth.Contracts;

public record UserResponse(string Id, string Email, string FirstName, string LastName, string? AvatarUrl, bool Verified);