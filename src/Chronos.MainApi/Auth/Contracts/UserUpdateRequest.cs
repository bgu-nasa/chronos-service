namespace Chronos.MainApi.Auth.Contracts;

public record UserUpdateRequest(string FirstName, string LastName, string? AvatarUrl);