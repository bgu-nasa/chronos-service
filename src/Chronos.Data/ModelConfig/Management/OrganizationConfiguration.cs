using Chronos.Data.Utils;
using Chronos.Domain.Management;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(Organization)));

        // Key config:
        builder.HasKey(o => o.Id);

        // Property config:
        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(d => d.Deleted)
            .HasDefaultValue(false);

        builder.Property(d => d.DeletedTime)
            .HasDefaultValue(0);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .IsRequired();
    }
}