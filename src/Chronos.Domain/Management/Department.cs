namespace Chronos.Domain.Management;

public class Department : ObjectInformation
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Name { get; set; }

    // Soft delete properties
    public bool Deleted { get; set; }
    public DateTime DeletedTime { get; set; }
}