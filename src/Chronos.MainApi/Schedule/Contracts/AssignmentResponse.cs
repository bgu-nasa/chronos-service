namespace Chronos.MainApi.Schedule.Contracts;

public record AssignmentResponse(
    string Id,
    string SlotId,
    string ResourceId,
    string ScheduledItemId);
