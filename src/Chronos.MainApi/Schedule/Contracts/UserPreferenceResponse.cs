namespace Chronos.MainApi.Schedule.Contracts;

public record UserPreferenceResponse(
    string Id,
    string UserId,
    string OrganizationId,
    string SchedulingPeriodId,
    string Key,
    string Value);
