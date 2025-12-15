namespace Chronos.Domain.Management;

public class Organization : ObjectInformation
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    // Soft delete properties
    public bool Deleted { get; set; }
    public DateTime DeletedTime { get; set; }
}