namespace Chronos.MainApi.Auth.Contracts;

public record UserPasswordUpdateRequest(string OldPassword, string NewPassword);