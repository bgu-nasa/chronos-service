namespace Chronos.MainApi.Schedule.Contracts;

public record UserConstraintResponse(
    string Id,
    string UserId,
    string OrganizationId,
    string SchedulingPeriodId,
    string Key,
    string Value);
