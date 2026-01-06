namespace Chronos.MainApi.Resources.Contracts;

public sealed record CreateResourceTypeRequest(
    Guid Id,
    Guid OrganizationId,
    string Type);