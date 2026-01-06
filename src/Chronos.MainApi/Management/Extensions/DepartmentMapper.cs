using Chronos.Domain.Management;
using Chronos.MainApi.Management.Contracts;

namespace Chronos.MainApi.Management.Extensions;

public static class DepartmentMapper
{
    public static DepartmentResponse ToDepartmentResponse(this Department department)
    {
        return new DepartmentResponse(department.Id, department.Name, department.Deleted);
    }
}