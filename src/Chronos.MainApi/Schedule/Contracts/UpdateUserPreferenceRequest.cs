namespace Chronos.MainApi.Schedule.Contracts;

public record UpdateUserPreferenceRequest(
    Guid UserId,
    Guid SchedulingPeriodId,
    string Key,
    string Value);
