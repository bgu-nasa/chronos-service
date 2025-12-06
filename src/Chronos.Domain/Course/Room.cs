namespace Chronos.Domain.Course;

public class Room : ObjectInformation
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public string? Building { get; set; }
    public string? RoomNumber { get; set; }
    public int? Capacity { get; set; }
    public RoomType? Type { get; set; }
    public List<RoomFeature> Features { get; }
    public bool HybridSupport { get; set; }
}
