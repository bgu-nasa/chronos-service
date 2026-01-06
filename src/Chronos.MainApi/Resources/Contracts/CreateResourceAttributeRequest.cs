namespace Chronos.MainApi.Resources.Contracts;

public sealed record CreateResourceAttributeRequest(
    Guid Id,
    Guid OrganizationId,
    string Title,
    string? Description);