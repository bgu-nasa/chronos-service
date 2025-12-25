using Chronos.Data.Utils;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Chronos.Data.ModelConfig.Resources;
public class ResourceAttributeConfiguration : IEntityTypeConfiguration<ResourceAttribute>
{
    public void Configure(EntityTypeBuilder<ResourceAttribute> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(ResourceAttribute)));

        builder.HasKey(ra => ra.Id);

        builder.Property(ra => ra.OrganizationId)
            .IsRequired();

        builder.Property(ra => ra.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(ra => ra.Description)
            .HasMaxLength(1024);

        builder.HasIndex(ra => new { ra.OrganizationId })
            .IsUnique();
    }
}