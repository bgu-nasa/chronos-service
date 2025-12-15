using Chronos.Data.Utils;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig.Resources;

public class RoomFeatureAssignmentConfiguration : IEntityTypeConfiguration<RoomFeatureAssignment>
{
    public void Configure(EntityTypeBuilder<RoomFeatureAssignment> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(RoomFeatureAssignment)));

        builder.HasKey(rfa => new { rfa.RoomId, rfa.RoomFeatureId });
    }
}

