namespace Chronos.Domain.Management;

public class DepartmentMembership : ObjectInformation
{
    public Guid DepartmentId { get; set; }
    public Guid UserId { get; set; }
}
