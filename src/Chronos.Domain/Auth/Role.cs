namespace Chronos.Domain.Auth;

public class Role : ObjectInformation
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
