namespace Chronos.Domain.Resources;

public class RoomFeatureAssignment : ObjectInformation
{
    public Guid RoomId { get; set; }
    public Guid RoomFeatureId { get; set; }
}
