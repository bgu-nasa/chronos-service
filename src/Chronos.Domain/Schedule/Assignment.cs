namespace Chronos.Domain.Schedule;

public class Assignment : ObjectInformation
{
    public Guid Id { get; set; }
    public Guid SlotId { get; set; }
    public Guid RoomId { get; set; }
    public Guid CourseActivityId { get; set; }
}
