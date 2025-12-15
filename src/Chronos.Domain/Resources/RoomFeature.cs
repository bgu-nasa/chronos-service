namespace Chronos.Domain.Resources;

public class RoomFeature : ObjectInformation
{
    public Guid Id { get; set; }
    public required string Feature { get; set; }
}
