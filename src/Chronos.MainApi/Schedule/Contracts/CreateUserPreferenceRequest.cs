namespace Chronos.MainApi.Schedule.Contracts;

public record CreateUserPreferenceRequest(
    Guid UserId,
    Guid SchedulingPeriodId,
    string Key,
    string Value);
