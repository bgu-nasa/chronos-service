namespace Chronos.Domain.Resources;

public class Room : ObjectInformation
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid RoomTypeId { get; set; }
    public required string Building { get; set; }
    public required string RoomNumber { get; set; }
    public int? Capacity { get; set; }
}
