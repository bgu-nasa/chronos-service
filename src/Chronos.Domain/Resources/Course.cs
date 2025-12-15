namespace Chronos.Domain.Resources;

public class Course : ObjectInformation
{
    public Guid Id { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid SchedulingPeriodId { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
}
