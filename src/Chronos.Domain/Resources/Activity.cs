namespace Chronos.Domain.Resources;

public class Activity : ObjectInformation
{
    public Guid Id { get; set; }
    public Guid CourseInstanceId { get; set; }
    public Guid LecturerUserId { get; set; }
    public required string ActivityType { get; set; }
    public int? ExpectedStudents { get; set; }
}
