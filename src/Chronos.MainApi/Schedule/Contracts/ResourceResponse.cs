namespace Chronos.MainApi.Schedule.Contracts;

public sealed record ExternalResourceResponse(
    Guid Id,
    Guid OrganizationId,
    Guid ResourceTypeId,
    string Location,
    string Identifier,
    int? Capacity);