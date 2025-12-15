using Chronos.Data.Utils;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig.Resources;

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(Resource)));

        builder.HasKey(r => r.Id);

        builder.Property(r => r.OrganizationId)
            .IsRequired();

        builder.Property(r => r.ResourceTypeId)
            .IsRequired();

        builder.Property(r => r.Location)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Identifier)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Capacity);

        builder.HasIndex(r => new { r.OrganizationId, r.Location, r.Identifier })
            .IsUnique();
    }
}

