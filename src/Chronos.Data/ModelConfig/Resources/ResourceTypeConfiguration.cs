using Chronos.Data.Utils;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Chronos.Data.ModelConfig.Resources;
public class ResourceTypeConfiguration : IEntityTypeConfiguration<ResourceType>
{
    public void Configure(EntityTypeBuilder<ResourceType> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(ResourceType)));

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.OrganizationId)
            .IsRequired();

        builder.Property(rt => rt.Type)
            .IsRequired()
            .HasMaxLength(128);
    }
}