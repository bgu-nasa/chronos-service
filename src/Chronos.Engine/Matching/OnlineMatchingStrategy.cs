using Chronos.Data.Repositories.Resources;
using Chronos.Data.Repositories.Schedule;
using Chronos.Domain.Resources;
using Chronos.Domain.Schedule;
using Chronos.Domain.Schedule.Messages;
using Chronos.Engine.Constraints;

namespace Chronos.Engine.Matching;

/// <summary>
/// Online mode matching for mid-semester constraint changes
/// Uses greedy re-matching with minimal disruption
/// </summary>
public class OnlineMatchingStrategy : IMatchingStrategy
{
    public SchedulingMode Mode => SchedulingMode.Online;

    private readonly IActivityConstraintRepository _activityConstraintRepository;
    private readonly IActivityRepository _activityRepository;
    private readonly ISlotRepository _slotRepository;
    private readonly IResourceRepository _resourceRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IConstraintProcessor _constraintProcessor;
    private readonly PreferenceWeightedRanker _ranker;
    private readonly ILogger<OnlineMatchingStrategy> _logger;

    public OnlineMatchingStrategy(
        IActivityConstraintRepository activityConstraintRepository,
        IActivityRepository activityRepository,
        ISlotRepository slotRepository,
        IResourceRepository resourceRepository,
        IAssignmentRepository assignmentRepository,
        IConstraintProcessor constraintProcessor,
        PreferenceWeightedRanker ranker,
        ILogger<OnlineMatchingStrategy> logger)
    {
        _activityConstraintRepository = activityConstraintRepository;
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
        if (request is not HandleConstraintChangeRequest constraintRequest)
        {
            throw new ArgumentException($"Expected HandleConstraintChangeRequest, got {request.GetType().Name}");
        }

        _logger.LogInformation(
            "Starting Online Matching for constraint {ConstraintId} in period {PeriodId}",
            constraintRequest.ActivityConstraintId,
            constraintRequest.SchedulingPeriodId);

        try
        {
            // Step 1: Load the new constraint
            var newConstraint = await _activityConstraintRepository.GetByIdAsync(
                constraintRequest.ActivityConstraintId);

            if (newConstraint == null)
            {
                _logger.LogError(
                    "Constraint {ConstraintId} not found",
                    constraintRequest.ActivityConstraintId);

                return new SchedulingResult(
                    constraintRequest.ActivityConstraintId,
                    false,
                    0,
                    0,
                    new List<Guid>(),
                    "Constraint not found");
            }

            _logger.LogInformation(
                "Processing constraint {ConstraintKey}={ConstraintValue} for Activity {ActivityId}",
                newConstraint.Key,
                newConstraint.Value,
                newConstraint.ActivityId);

            // Step 2: Get affected activity
            var activity = await _activityRepository.GetByIdAsync(newConstraint.ActivityId);
            if (activity == null)
            {
                _logger.LogError("Activity {ActivityId} not found", newConstraint.ActivityId);
                return new SchedulingResult(
                    constraintRequest.ActivityConstraintId,
                    false,
                    0,
                    0,
                    new List<Guid>(),
                    "Activity not found");
            }

            // Step 3: Get current assignment for this activity
            var currentAssignments = await _assignmentRepository.GetByActivityIdAsync(activity.Id);
            var currentAssignment = currentAssignments.FirstOrDefault();

            if (currentAssignment == null)
            {
                _logger.LogInformation(
                    "Activity {ActivityId} has no current assignment. No action needed.",
                    activity.Id);

                return new SchedulingResult(
                    constraintRequest.ActivityConstraintId,
                    true,
                    0,
                    0,
                    new List<Guid>(),
                    "Activity not currently assigned");
            }

            // Step 4: Check if current assignment is still valid
            var excludedSlots = await _constraintProcessor.GetExcludedSlotIdsAsync(
                activity.Id,
                constraintRequest.OrganizationId);

            bool isCurrentAssignmentValid = !excludedSlots.Contains(currentAssignment.SlotId);

            if (isCurrentAssignmentValid)
            {
                _logger.LogInformation(
                    "Current assignment for Activity {ActivityId} is still valid. No changes needed.",
                    activity.Id);

                return new SchedulingResult(
                    constraintRequest.ActivityConstraintId,
                    true,
                    0,
                    0,
                    new List<Guid>(),
                    "Current assignment remains valid");
            }

            _logger.LogWarning(
                "Current assignment for Activity {ActivityId} violates new constraint. Re-matching...",
                activity.Id);

            // Step 5: Re-match the activity
            var result = await RematchActivityAsync(
                activity,
                currentAssignment,
                excludedSlots,
                constraintRequest.OrganizationId,
                constraintRequest.SchedulingPeriodId,
                cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Online Matching");
            return new SchedulingResult(
                constraintRequest.ActivityConstraintId,
                false,
                0,
                0,
                new List<Guid>(),
                $"Algorithm failed: {ex.Message}");
        }
    }

    private async Task<SchedulingResult> RematchActivityAsync(
        Domain.Resources.Activity activity,
        Assignment currentAssignment,
        HashSet<Guid> excludedSlots,
        Guid organizationId,
        Guid schedulingPeriodId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Re-matching Activity {ActivityId}", activity.Id);

        // Step a: Delete current assignment (free up the slot-resource pair)
        await _assignmentRepository.DeleteAsync(currentAssignment);
        _logger.LogInformation(
            "Deleted current assignment for Activity {ActivityId}",
            activity.Id);

        // Step b: Load all current assignments for the scheduling period
        var allAssignments = await _assignmentRepository.GetAllAsync();
        var occupiedPairs = allAssignments
            .Select(a => (a.SlotId, a.ResourceId))
            .ToHashSet();

        _logger.LogDebug(
            "Found {OccupiedCount} occupied (Slot, Resource) pairs",
            occupiedPairs.Count);

        // Step c: Get available (Slot, Resource) pairs
        var slots = await _slotRepository.GetBySchedulingPeriodIdAsync(schedulingPeriodId);
        var resources = await _resourceRepository.GetAllAsync();

        var availablePairs = new List<SlotResourcePair>();

        foreach (var slot in slots)
        {
            // Skip excluded slots
            if (excludedSlots.Contains(slot.Id))
                continue;

            foreach (var resource in resources)
            {
                // Skip occupied pairs
                if (occupiedPairs.Contains((slot.Id, resource.Id)))
                    continue;

                // Check capacity
                if (!IsCapacitySufficient(resource, activity.ExpectedStudents))
                    continue;

                availablePairs.Add(new SlotResourcePair(slot, resource));
            }
        }

        _logger.LogInformation(
            "Found {AvailableCount} available (Slot, Resource) pairs for Activity {ActivityId}",
            availablePairs.Count,
            activity.Id);

        // Step d: If no valid pairs exist, fail
        if (!availablePairs.Any())
        {
            _logger.LogError(
                "No available (Slot, Resource) pairs for Activity {ActivityId}. Manual intervention required.",
                activity.Id);

            return new SchedulingResult(
                activity.Id,
                false,
                0,
                1, // 1 assignment was removed
                new List<Guid> { activity.Id },
                "No valid slot-resource pairs available");
        }

        // Step e: Apply preference-weighted random selection
        var weights = new double[availablePairs.Count];
        for (int i = 0; i < availablePairs.Count; i++)
        {
            weights[i] = await _ranker.CalculateWeightAsync(
                availablePairs[i],
                activity.AssignedUserId,
                organizationId,
                schedulingPeriodId);
        }

        var selected = _ranker.SelectRandomWeighted(availablePairs, weights);

        _logger.LogInformation(
            "Selected new assignment for Activity {ActivityId}: {Candidate}",
            activity.Id,
            selected);

        // Step f: Create new assignment
        var newAssignment = new Assignment
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            SlotId = selected.SlotId,
            ResourceId = selected.ResourceId,
            ActivityId = activity.Id
        };

        await _assignmentRepository.AddAsync(newAssignment);

        _logger.LogInformation(
            "Successfully re-matched Activity {ActivityId}",
            activity.Id);

        return new SchedulingResult(
            activity.Id,
            true,
            0, // No new assignments (replacement)
            1, // 1 assignment modified
            new List<Guid>(),
            "Activity successfully re-matched");
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
