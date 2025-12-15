using Chronos.Data.Utils;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig.Resources;

public class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
{
    public void Configure(EntityTypeBuilder<RoomType> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(RoomType)));

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Type)
            .IsRequired()
            .HasMaxLength(100);
    }
}

