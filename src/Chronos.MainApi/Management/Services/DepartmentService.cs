using Chronos.Data.Repositories.Management;
using Chronos.Domain.Management;
using Chronos.Shared.Exceptions;

namespace Chronos.MainApi.Management.Services;

public class DepartmentService(
    IDepartmentRepository departmentRepository,
    IOrganizationRepository organizationRepository,
    ILogger<DepartmentService> logger) : IDepartmentService
{
    public async Task<Guid> CreateDepartmentAsync(Guid organizationId, string name)
    {
        logger.LogInformation("Creating department. OrganizationId: {OrganizationId}, Name: {Name}", organizationId, name);
        
        await ValidateOrganizationAsync(organizationId);

        var department = new Department
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            Name = name,
            Deleted = false
        };

        await departmentRepository.AddAsync(department);
        
        logger.LogInformation("Department created successfully. DepartmentId: {DepartmentId}, OrganizationId: {OrganizationId}", department.Id, organizationId);
        return department.Id;
    }

    public async Task<Department> GetDepartmentAsync(Guid organizationId, Guid departmentId)
    {
        logger.LogDebug("Retrieving department. OrganizationId: {OrganizationId}, DepartmentId: {DepartmentId}", organizationId, departmentId);
        
        await ValidateOrganizationAsync(organizationId);
        var department = await ValidateAndGetDepartmentAsync(organizationId, departmentId, excludeDeleted: true);
        return department;
    }

    public async Task<List<Department>> GetDepartmentsAsync(Guid organizationId)
    {
        logger.LogDebug("Retrieving all departments for organization. OrganizationId: {OrganizationId}", organizationId);
        
        await ValidateOrganizationAsync(organizationId);

        var allDepartments = await departmentRepository.GetAllAsync();
        var filteredDepartments = allDepartments
            .Where(d => d.OrganizationId == organizationId && !d.Deleted)
            .ToList();
        
        logger.LogDebug("Retrieved {Count} departments for organization. OrganizationId: {OrganizationId}", filteredDepartments.Count, organizationId);
        return filteredDepartments;
    }

    public async Task UpdateDepartmentAsync(Guid organizationId, Guid departmentId, string name)
    {
        logger.LogInformation("Updating department. OrganizationId: {OrganizationId}, DepartmentId: {DepartmentId}, NewName: {Name}", organizationId, departmentId, name);
        
        await ValidateOrganizationAsync(organizationId);
        var department = await ValidateAndGetDepartmentAsync(organizationId, departmentId, excludeDeleted: true);

        department.Name = name;
        await departmentRepository.UpdateAsync(department);
        
        logger.LogInformation("Department updated successfully. DepartmentId: {DepartmentId}", departmentId);
    }

    public async Task SetForDeletionAsync(Guid organizationId, Guid departmentId)
    {
        logger.LogInformation("Setting department for deletion. OrganizationId: {OrganizationId}, DepartmentId: {DepartmentId}", organizationId, departmentId);
        
        await ValidateOrganizationAsync(organizationId);
        var department = await ValidateAndGetDepartmentAsync(organizationId, departmentId, excludeDeleted: false);

        if (department.Deleted)
        {
            logger.LogWarning("Department already set for deletion. DepartmentId: {DepartmentId}", departmentId);
            throw new BadRequestException("Department is already set for deletion");
        }

        department.Deleted = true;
        department.DeletedTime = DateTime.UtcNow;
        await departmentRepository.UpdateAsync(department);
        
        logger.LogInformation("Department set for deletion successfully. DepartmentId: {DepartmentId}", departmentId);
    }

    public async Task RestoreDeletedDepartmentAsync(Guid organizationId, Guid departmentId)
    {
        logger.LogInformation("Restoring deleted department. OrganizationId: {OrganizationId}, DepartmentId: {DepartmentId}", organizationId, departmentId);
        
        await ValidateOrganizationAsync(organizationId);
        var department = await ValidateAndGetDepartmentAsync(organizationId, departmentId, excludeDeleted: false);

        if (!department.Deleted)
        {
            logger.LogWarning("Department is not set for deletion. DepartmentId: {DepartmentId}", departmentId);
            throw new BadRequestException("Department is not set for deletion");
        }

        department.Deleted = false;
        department.DeletedTime = default;
        await departmentRepository.UpdateAsync(department);
        
        logger.LogInformation("Department restored successfully. DepartmentId: {DepartmentId}", departmentId);
    }

    private async Task ValidateOrganizationAsync(Guid organizationId)
    {
        var organization = await organizationRepository.GetByIdAsync(organizationId);
        
        if (organization == null || organization.Deleted)
        {
            logger.LogWarning("Organization not found or deleted. OrganizationId: {OrganizationId}", organizationId);
            throw new BadRequestException("Organization not found");
        }
    }

    private async Task<Department> ValidateAndGetDepartmentAsync(Guid organizationId, Guid departmentId, bool excludeDeleted)
    {
        var department = await departmentRepository.GetByIdAsync(departmentId);
        
        if (department == null || department.OrganizationId != organizationId)
        {
            logger.LogWarning("Department not found or does not belong to organization. DepartmentId: {DepartmentId}, OrganizationId: {OrganizationId}", departmentId, organizationId);
            throw new BadRequestException("Department not found");
        }

        if (excludeDeleted && department.Deleted)
        {
            logger.LogWarning("Department is deleted. DepartmentId: {DepartmentId}", departmentId);
            throw new BadRequestException("Department not found");
        }

        return department;
    }
}