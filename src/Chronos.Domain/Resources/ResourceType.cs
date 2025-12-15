namespace Chronos.Domain.Resources;

public class ResourceType : ObjectInformation
{
    public Guid Id { get; set; }
    public required string Type { get; set; }
}
