namespace Chronos.Domain.Resources;

public class Resource : ObjectInformation
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid ResourceTypeId { get; set; }
    public required string Location { get; set; }
    public required string Identifier { get; set; }
    public int? Capacity { get; set; }
}
