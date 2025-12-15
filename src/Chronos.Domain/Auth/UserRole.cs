namespace Chronos.Domain.Auth;

public class UserRole : ObjectInformation
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
