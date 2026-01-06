using Chronos.Data.Utils;
using Chronos.Domain.Management;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(Department)));

        // Key config:
        builder.HasKey(d => d.Id);

        // Property config:
        builder.Property(d => d.OrganizationId)
            .IsRequired();

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(d => d.Deleted)
            .HasDefaultValue(false);

        builder.Property(d => d.DeletedTime)
            .HasDefaultValue(DateTime.MinValue);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .IsRequired();
    }
}