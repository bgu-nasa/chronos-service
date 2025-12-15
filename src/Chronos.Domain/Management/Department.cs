namespace Chronos.Domain.Management;

public class Department : ObjectInformation
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public required string Name { get; set; }
}
