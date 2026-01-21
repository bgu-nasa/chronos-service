namespace Chronos.MainApi.Auth.Contracts;

public record RegisterRequest(CreateUserRequest AdminUser, string OrganizationName, string Plan, string InviteCode);