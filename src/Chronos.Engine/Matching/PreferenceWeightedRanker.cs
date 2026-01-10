using Chronos.Data.Repositories.Schedule;
using Chronos.Domain.Schedule;

namespace Chronos.Engine.Matching;

/// <summary>
/// Calculates preference weights for (Slot, Resource) pairs and performs weighted random selection
/// </summary>
public class PreferenceWeightedRanker
{
    private readonly IUserPreferenceRepository _userPreferenceRepository;
    private readonly ILogger<PreferenceWeightedRanker> _logger;
    private readonly Random _random = new();

    public PreferenceWeightedRanker(
        IUserPreferenceRepository userPreferenceRepository,
        ILogger<PreferenceWeightedRanker> logger)
    {
        _userPreferenceRepository = userPreferenceRepository;
        _logger = logger;
    }

    /// <summary>
    /// Calculate preference weight for a (Slot, Resource) pair based on user preferences
    /// </summary>
    public async Task<double> CalculateWeightAsync(
        SlotResourcePair candidate,
        Guid userId,
        Guid organizationId,
        Guid schedulingPeriodId)
    {
        var preferences = await _userPreferenceRepository
            .GetByUserPeriodAsync(userId, schedulingPeriodId);

        if (!preferences.Any())
        {
            return 1.0; // Neutral weight if no preferences
        }

        double weight = 1.0;

        foreach (var pref in preferences)
        {
            if (CandidateMatchesPreference(candidate, pref))
            {
                var multiplier = GetPreferenceMultiplier(pref.Key, pref.Value);
                weight *= multiplier;

                _logger.LogTrace(
                    "Applied preference {PreferenceKey}={PreferenceValue} with multiplier {Multiplier} to candidate {Candidate}",
                    pref.Key,
                    pref.Value,
                    multiplier,
                    candidate);
            }
        }

        return weight;
    }

    /// <summary>
    /// Select one candidate using weighted random sampling
    /// Higher weight = higher probability of selection
    /// </summary>
    public SlotResourcePair SelectRandomWeighted(
        List<SlotResourcePair> candidates,
        double[] weights)
    {
        if (candidates.Count == 0)
        {
            throw new ArgumentException("Cannot select from empty candidate list", nameof(candidates));
        }

        if (candidates.Count != weights.Length)
        {
            throw new ArgumentException("Candidates and weights must have same length");
        }

        if (candidates.Count == 1)
        {
            return candidates[0];
        }

        // Calculate cumulative weights
        double totalWeight = weights.Sum();

        if (totalWeight <= 0)
        {
            // All weights are zero or negative, fall back to uniform random
            _logger.LogWarning("All weights are non-positive, using uniform random selection");
            return candidates[_random.Next(candidates.Count)];
        }

        // Generate random value in [0, totalWeight)
        double randomValue = _random.NextDouble() * totalWeight;

        // Find the candidate corresponding to this random value
        double cumulative = 0;
        for (int i = 0; i < candidates.Count; i++)
        {
            cumulative += weights[i];
            if (randomValue < cumulative)
            {
                _logger.LogDebug(
                    "Selected candidate {Index} with weight {Weight} (cumulative: {Cumulative}, random: {Random})",
                    i,
                    weights[i],
                    cumulative,
                    randomValue);

                return candidates[i];
            }
        }

        // Fallback (should rarely happen due to floating point precision)
        return candidates[^1];
    }

    private bool CandidateMatchesPreference(SlotResourcePair candidate, UserPreference preference)
    {
        // Example matching logic - can be extended based on preference types
        return preference.Key switch
        {
            "preferred_weekday" => candidate.Slot.Weekday.Equals(preference.Value, StringComparison.OrdinalIgnoreCase),
            "avoid_weekday" => candidate.Slot.Weekday.Equals(preference.Value, StringComparison.OrdinalIgnoreCase),
            "preferred_time_morning" => candidate.Slot.FromTime.Hours < 12,
            "preferred_time_afternoon" => candidate.Slot.FromTime.Hours >= 12 && candidate.Slot.FromTime.Hours < 17,
            "preferred_time_evening" => candidate.Slot.FromTime.Hours >= 17,
            _ => false
        };
    }

    private double GetPreferenceMultiplier(string key, string value)
    {
        // Weight multipliers based on preference type
        return key switch
        {
            "preferred_weekday" => 3.0,
            "preferred_time_morning" => 3.0,
            "preferred_time_afternoon" => 2.0,
            "preferred_time_evening" => 2.0,
            "avoid_weekday" => 0.3,
            "avoid_time_morning" => 0.3,
            "avoid_time_afternoon" => 0.5,
            "avoid_time_evening" => 0.5,
            _ => 1.0 // Neutral for unknown preferences
        };
    }
}
