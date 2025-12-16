namespace Chronos.Domain.Schedule;

public class Slot : ObjectInformation
{
    public Guid Id { get; set; }
    public Guid SchedulingPeriodId { get; set; }
    public required string Weekday { get; set; }
    public TimeSpan FromTime { get; set; }
    public TimeSpan ToTime { get; set; }
}
