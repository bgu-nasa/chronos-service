namespace Chronos.Domain.Course;

public class CourseActivity : ObjectInformation
{
    public Guid Id { get; }
    public Guid CourseId { get; }
    public Guid InstructorId { get; }
    public CourseActivityType Type { get; set; }
    public int DurationMinutes { get; set; }
}
