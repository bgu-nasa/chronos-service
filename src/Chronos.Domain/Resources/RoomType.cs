namespace Chronos.Domain.Resources;

public class RoomType : ObjectInformation
{
    public Guid Id { get; set; }
    public required string Type { get; set; }
}
