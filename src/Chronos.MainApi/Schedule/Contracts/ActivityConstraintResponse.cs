namespace Chronos.MainApi.Schedule.Contracts;

public record ActivityConstraintResponse(
    string Id,
    string ActivityId,
    string OrganizationId,
    string Key,
    string Value);
