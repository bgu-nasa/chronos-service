namespace Chronos.Domain.Schedule;

public class OrganizationPolicy : ObjectInformation
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid SchedulingPeriodId { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }
}
