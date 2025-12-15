namespace Chronos.Domain.Management;

public class Organization : ObjectInformation
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
