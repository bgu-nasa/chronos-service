namespace Chronos.Domain.Resources;

public class ResourceAttribute : ObjectInformation
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
}
