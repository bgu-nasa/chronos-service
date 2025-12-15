using Chronos.Data.Utils;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig.Resources;

public class RoomFeatureConfiguration : IEntityTypeConfiguration<RoomFeature>
{
    public void Configure(EntityTypeBuilder<RoomFeature> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(RoomFeature)));

        builder.HasKey(rf => rf.Id);

        builder.Property(rf => rf.Feature)
            .IsRequired()
            .HasMaxLength(256);
    }
}

