namespace Chronos.MainApi.Resources.Contracts;

public record ResourceAttributeResponse(
    Guid Id,
    Guid OrganizationId,
    string Title,
    string? Description);