namespace Chronos.Domain.Schedule;

public class SchedulingPeriod : ObjectInformation
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public required string Name { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}
