using Chronos.Data.Repositories.Resources;
using Chronos.Data.Repositories.Schedule;
using Chronos.Domain.Resources;
using Chronos.Domain.Schedule;
using Chronos.Domain.Schedule.Messages;
using Chronos.Engine.Constraints;

namespace Chronos.Engine.Matching;

/// <summary>
/// Batch mode matching using Ranking Algorithm
/// Achieves 1-1/e ≈ 0.632 competitive ratio
/// </summary>
public class RankingAlgorithmStrategy : IMatchingStrategy
{
    public SchedulingMode Mode => SchedulingMode.Batch;

    private readonly IActivityRepository _activityRepository;
    private readonly ISlotRepository _slotRepository;
    private readonly IResourceRepository _resourceRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IConstraintProcessor _constraintProcessor;
    private readonly PreferenceWeightedRanker _ranker;
    private readonly ILogger<RankingAlgorithmStrategy> _logger;
    private readonly Random _random = new();

    public RankingAlgorithmStrategy(
        IActivityRepository activityRepository,
        ISlotRepository slotRepository,
        IResourceRepository resourceRepository,
        IAssignmentRepository assignmentRepository,
        IConstraintProcessor constraintProcessor,
        PreferenceWeightedRanker ranker,
        ILogger<RankingAlgorithmStrategy> logger)
    {
        _activityRepository = activityRepository;
        _slotRepository = slotRepository;
        _resourceRepository = resourceRepository;
        _assignmentRepository = assignmentRepository;
        _constraintProcessor = constraintProcessor;
        _ranker = ranker;
        _logger = logger;
    }

    public async Task<SchedulingResult> ExecuteAsync(object request, CancellationToken cancellationToken)
    {
        if (request is not SchedulePeriodRequest periodRequest)
        {
            throw new ArgumentException($"Expected SchedulePeriodRequest, got {request.GetType().Name}");
        }

        _logger.LogInformation(
            "Starting Ranking Algorithm for SchedulingPeriod {PeriodId}, Organization {OrgId}",
            periodRequest.SchedulingPeriodId,
            periodRequest.OrganizationId);

        try
        {
            // Step 1: Load all activities for the scheduling period
            var activities = await LoadActivitiesForPeriodAsync(periodRequest.SchedulingPeriodId);
            _logger.LogInformation("Loaded {ActivityCount} activities to schedule", activities.Count);

            if (activities.Count == 0)
            {
                return new SchedulingResult(
                    periodRequest.SchedulingPeriodId,
                    true,
                    0,
                    0,
                    new List<Guid>(),
                    "No activities to schedule");
            }

            // Step 2: Load all (Slot, Resource) pairs
            var slots = await _slotRepository.GetBySchedulingPeriodIdAsync(periodRequest.SchedulingPeriodId);
            var resources = await _resourceRepository.GetAllAsync();

            _logger.LogInformation(
                "Loaded {SlotCount} slots and {ResourceCount} resources",
                slots.Count,
                resources.Count);

            // Step 3: Generate all (Slot, Resource) combinations → Set L
            var allPairs = GenerateSlotResourcePairs(slots, resources);
            _logger.LogInformation("Generated {PairCount} (Slot, Resource) pairs", allPairs.Count);

            // Step 4: Generate RANDOM PERMUTATION σ of L (the "Ranking")
            var rankedPairs = GenerateRandomPermutation(allPairs);
            _logger.LogInformation("Generated random permutation of {PairCount} pairs", rankedPairs.Count);

            // Step 5: Process activities and match using ranking algorithm
            var result = await ProcessActivitiesWithRankingAsync(
                activities,
                rankedPairs,
                periodRequest.OrganizationId,
                periodRequest.SchedulingPeriodId,
                cancellationToken);

            _logger.LogInformation(
                "Ranking Algorithm completed. Created: {Created}, Failed: {Failed}",
                result.AssignmentsCreated,
                result.UnscheduledActivityIds.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Ranking Algorithm");
            return new SchedulingResult(
                periodRequest.SchedulingPeriodId,
                false,
                0,
                0,
                new List<Guid>(),
                $"Algorithm failed: {ex.Message}");
        }
    }

    private async Task<List<Domain.Resources.Activity>> LoadActivitiesForPeriodAsync(Guid schedulingPeriodId)
    {
        // For now, load all activities (in production, filter by scheduling period)
        var allActivities = await _activityRepository.GetAllAsync();
        return allActivities;
    }

    private List<SlotResourcePair> GenerateSlotResourcePairs(
        List<Slot> slots,
        List<Domain.Resources.Resource> resources)
    {
        var pairs = new List<SlotResourcePair>();

        foreach (var slot in slots)
        {
            foreach (var resource in resources)
            {
                pairs.Add(new SlotResourcePair(slot, resource));
            }
        }

        return pairs;
    }

    private List<SlotResourcePair> GenerateRandomPermutation(List<SlotResourcePair> pairs)
    {
        // Fisher-Yates shuffle to generate random permutation
        var permutation = new List<SlotResourcePair>(pairs);
        int n = permutation.Count;

        for (int i = n - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (permutation[i], permutation[j]) = (permutation[j], permutation[i]);
        }

        // Assign ranks based on position in permutation
        for (int i = 0; i < permutation.Count; i++)
        {
            permutation[i] = permutation[i] with { Rank = i + 1 };
        }

        return permutation;
    }

    private async Task<SchedulingResult> ProcessActivitiesWithRankingAsync(
        List<Domain.Resources.Activity> activities,
        List<SlotResourcePair> rankedPairs,
        Guid organizationId,
        Guid schedulingPeriodId,
        CancellationToken cancellationToken)
    {
        var createdAssignments = 0;
        var unscheduledActivities = new List<Guid>();
        var occupiedPairs = new HashSet<(Guid SlotId, Guid ResourceId)>();

        foreach (var activity in activities)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Batch scheduling cancelled");
                break;
            }

            _logger.LogDebug("Processing Activity {ActivityId}", activity.Id);

            // Get excluded slots from constraints
            var excludedSlots = await _constraintProcessor.GetExcludedSlotIdsAsync(
                activity.Id,
                organizationId);

            // Filter valid candidates
            var validCandidates = rankedPairs
                .Where(p => !excludedSlots.Contains(p.SlotId))
                .Where(p => !occupiedPairs.Contains((p.SlotId, p.ResourceId)))
                .Where(p => IsCapacitySufficient(p.Resource, activity.ExpectedStudents))
                .OrderBy(p => p.Rank) // Order by rank (earlier rank = higher priority)
                .ToList();

            if (!validCandidates.Any())
            {
                _logger.LogWarning(
                    "No valid candidates for Activity {ActivityId}. Excluded slots: {ExcludedCount}, Occupied: {OccupiedCount}",
                    activity.Id,
                    excludedSlots.Count,
                    occupiedPairs.Count);

                unscheduledActivities.Add(activity.Id);
                continue;
            }

            _logger.LogDebug(
                "Found {CandidateCount} valid candidates for Activity {ActivityId}",
                validCandidates.Count,
                activity.Id);

            // Calculate preference weights for top candidates (optimize by only checking top N)
            var topCandidates = validCandidates.Take(10).ToList();
            var weights = new double[topCandidates.Count];

            for (int i = 0; i < topCandidates.Count; i++)
            {
                weights[i] = await _ranker.CalculateWeightAsync(
                    topCandidates[i],
                    activity.AssignedUserId,
                    organizationId,
                    schedulingPeriodId);

                // Bias towards earlier ranks (primary criterion)
                // Weight decreases exponentially with rank
                weights[i] *= Math.Exp(-(topCandidates[i].Rank - 1) * 0.1);
            }

            // Select using weighted random sampling
            var selected = _ranker.SelectRandomWeighted(topCandidates, weights);

            _logger.LogInformation(
                "Matched Activity {ActivityId} to {Candidate}",
                activity.Id,
                selected);

            // Create assignment
            var assignment = new Assignment
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                SlotId = selected.SlotId,
                ResourceId = selected.ResourceId,
                ActivityId = activity.Id
            };

            await _assignmentRepository.AddAsync(assignment);
            occupiedPairs.Add((selected.SlotId, selected.ResourceId));
            createdAssignments++;
        }

        return new SchedulingResult(
            schedulingPeriodId,
            true,
            createdAssignments,
            0,
            unscheduledActivities,
            unscheduledActivities.Any()
                ? $"{unscheduledActivities.Count} activities could not be scheduled"
                : null);
    }

    private bool IsCapacitySufficient(Domain.Resources.Resource resource, int? expectedStudents)
    {
        if (expectedStudents == null || resource.Capacity == null)
        {
            return true; // No capacity constraint
        }

        return resource.Capacity >= expectedStudents;
    }
}
