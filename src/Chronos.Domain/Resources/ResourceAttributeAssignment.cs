namespace Chronos.Domain.Resources;

public class ResourceAttributeAssignment : ObjectInformation
{
    public Guid ResourceId { get; set; }
    public Guid ResourceAttributeId { get; set; }
}
