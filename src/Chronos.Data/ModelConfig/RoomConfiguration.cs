using Chronos.Data.Utils;
using Chronos.Domain.Course;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(Room)));

        // Key config:
        builder.HasKey(r => r.Id);

        // Property config:
        builder.Property(r => r.OrganizationId).IsRequired();
        builder.Property(r => r.Building).HasMaxLength(256);
        builder.Property(r => r.RoomNumber).HasMaxLength(50);
        builder.Property(r => r.Capacity);
        builder.Property(r => r.Type);
        builder.Property(r => r.HybridSupport).IsRequired();

        // Store Features as JSON
        builder
            .Property(r => r.Features)
            .HasConversion(
                v =>
                    System.Text.Json.JsonSerializer.Serialize(
                        v,
                        (System.Text.Json.JsonSerializerOptions)null
                    ),
                v =>
                    System.Text.Json.JsonSerializer.Deserialize<List<RoomFeature>>(
                        v,
                        (System.Text.Json.JsonSerializerOptions)null
                    )
                    ?? new List<RoomFeature>()
            )
            .HasColumnType("jsonb");

        // ObjectInformation base class properties
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.UpdatedAt).IsRequired();

        // Indexes for performance
        builder.HasIndex(r => r.OrganizationId);
        builder.HasIndex(r => r.Building);
        builder.HasIndex(r => r.Type);
    }
}
