using Chronos.Data.Utils;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Chronos.Data.ModelConfig.Resources;
public class ResourceAttributeAssignmentConfiguration : IEntityTypeConfiguration<ResourceAttributeAssignment>
{
    public void Configure(EntityTypeBuilder<ResourceAttributeAssignment> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(ResourceAttributeAssignment)));

        builder.Property(ra => ra.OrganizationId)
            .IsRequired();

        builder.HasKey(raa => new { raa.ResourceId, raa.ResourceAttributeId });

        builder.HasIndex(raa => new { raa.OrganizationId })
            .IsUnique();
    }
}