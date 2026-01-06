using Chronos.Data.Utils;
using Chronos.Domain.Management.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig;

public class RoleAssignmentConfiguration : IEntityTypeConfiguration<RoleAssignment>
{
    public void Configure(EntityTypeBuilder<RoleAssignment> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(RoleAssignment)));

        // Key config:
        builder.HasKey(ra => ra.Id);

        // Property config:
        builder.Property(ra => ra.UserId)
            .IsRequired();

        builder.Property(ra => ra.OrganizationId)
            .IsRequired();

        builder.Property(ra => ra.DepartmentId);

        builder.Property(ra => ra.Role)
            .IsRequired();

        // Index config:
        builder.HasIndex(ra => new { ra.UserId, ra.OrganizationId, ra.DepartmentId, ra.Role })
            .IsUnique();
    }
}