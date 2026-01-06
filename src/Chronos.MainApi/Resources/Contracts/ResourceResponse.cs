namespace Chronos.MainApi.Resources.Contracts;

public sealed record ResourceResponse(
    Guid Id,
    Guid OrganizationId,
    Guid ResourceTypeId,
    string Location,
    string Identifier,
    int? Capacity);