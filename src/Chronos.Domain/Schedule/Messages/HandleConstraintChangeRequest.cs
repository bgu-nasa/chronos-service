namespace Chronos.Domain.Schedule.Messages;

public record HandleConstraintChangeRequest(
    Guid ActivityConstraintId,
    Guid OrganizationId,
    Guid SchedulingPeriodId,
    SchedulingMode Mode = SchedulingMode.Online
);

