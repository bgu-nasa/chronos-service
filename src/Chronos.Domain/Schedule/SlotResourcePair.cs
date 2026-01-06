using Chronos.Domain.Resources;

namespace Chronos.Domain.Schedule;

/// <summary>
/// Represents a (Slot, Resource) pair in the bipartite matching graph
/// This is a node on the "Left" side (L) of the bipartite graph
/// </summary>
public record SlotResourcePair(Slot Slot, Resource Resource)
{
    public Guid SlotId => Slot.Id;
    public Guid ResourceId => Resource.Id;

    /// <summary>
    /// Rank assigned by random permutation (used in Ranking algorithm)
    /// </summary>
    public int Rank { get; init; }

    /// <summary>
    /// Preference weight calculated based on user preferences
    /// </summary>
    public double PreferenceWeight { get; init; } = 1.0;

    public override string ToString() =>
        $"Slot: {Slot.Weekday} {Slot.FromTime}-{Slot.ToTime}, Resource: {Resource.Identifier} (Rank: {Rank}, Weight: {PreferenceWeight:F2})";
}
