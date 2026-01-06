namespace Chronos.MainApi.Resources.Contracts;

public sealed record ResourceTypeResponse(
    Guid Id,
    Guid OrganizationId,
    string Type);