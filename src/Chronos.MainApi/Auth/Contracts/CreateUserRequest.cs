namespace Chronos.MainApi.Auth.Contracts;

public record CreateUserRequest(string Email, string FirstName, string LastName);