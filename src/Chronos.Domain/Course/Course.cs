namespace Chronos.Domain.Course;

public class Course : ObjectInformation
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public Guid DepartmentId { get; }
    public Guid SchedulingPeriodId { get; }
    public string Code { get; }
    public string Name { get; }
    public string? Description { get; }
    public int EstimatedEnrollment { get; }

    // Navigation properties
    public ICollection<CourseActivity> Activities { get; } = new List<CourseActivity>();
}
