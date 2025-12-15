using Chronos.Data.Utils;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig.Resources;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(Room)));

        builder.HasKey(r => r.Id);

        builder.Property(r => r.OrganizationId)
            .IsRequired();

        builder.Property(r => r.RoomTypeId)
            .IsRequired();

        builder.Property(r => r.Building)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.RoomNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Capacity);

        builder.HasIndex(r => new { r.OrganizationId, r.Building, r.RoomNumber })
            .IsUnique();
    }
}

