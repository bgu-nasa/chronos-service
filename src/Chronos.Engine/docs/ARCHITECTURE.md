# Chronos Scheduling Engine - Architecture Documentation

## Executive Summary

The Chronos Scheduling Engine implements a dual-mode asynchronous scheduling system for educational institutions. It solves the NP-hard problem of optimally assigning Activities to (Slot, Resource) pairs while respecting hard constraints and soft preferences.

The system is based on the **online bipartite matching algorithm** (Ranking Algorithm) from computer science theory, achieving a competitive ratio of **1-1/e ≈ 0.632** for batch scheduling.

## Problem Definition

### Bipartite Graph Model

```
Left Side (L):  (Slot, Resource) pairs
                - Slot: Time block (Weekday, FromTime, ToTime)
                - Resource: Physical location with capacity

Right Side (R): Activities
                - Course sessions needing scheduling
                - Assigned to instructors
                - Expected student count

Edges:          Valid assignments respecting constraints

Matching:       One-to-one assignment of Activities to (Slot, Resource) pairs
```

### Constraints

**Hard Constraints** (must be satisfied):
- ActivityConstraints: "No Monday mornings", "Required capacity ≥ 50", etc.
- Capacity: Resource.Capacity ≥ Activity.ExpectedStudents
- Uniqueness: Each (Slot, Resource) pair can only be used once

**Soft Preferences** (influence but don't constrain):
- UserPreferences: "Prefer mornings", "Avoid Fridays", etc.
- Applied as probability weights in selection

## Architecture Overview

```
┌─────────────────┐
│  Chronos.MainApi│
│  (REST API)     │
└────────┬────────┘
         │ Publishes requests
         ▼
┌─────────────────┐
│   RabbitMQ      │
│  Message Broker │
├─────────────────┤
│  Batch Queue    │ ◄─── SchedulePeriodRequest
│  Online Queue   │ ◄─── HandleConstraintChangeRequest
└────────┬────────┘
         │ Consumes
         ▼
┌─────────────────┐
│ Chronos.Engine  │
│ (Worker Service)│
├─────────────────┤
│ ┌─────────────┐ │
│ │Orchestrator │ │
│ └──────┬──────┘ │
│        │        │
│ ┌──────▼──────┐ │
│ │  Matching   │ │
│ │ Strategies  │ │
│ ├─────────────┤ │
│ │ Ranking     │ │ Batch Mode
│ │ Online      │ │ Online Mode
│ └─────────────┘ │
└────────┬────────┘
         │ Writes
         ▼
┌─────────────────┐
│   Database      │
│  (Assignments)  │
└─────────────────┘
```

## Algorithm Implementations

### 1. Batch Mode: Ranking Algorithm

**Source**: Lecture 17, Section 2.2

**When Used**: Schedule entire SchedulingPeriod before semester starts

**Competitive Ratio**: 1-1/e ≈ 0.632 (best-known for randomized online bipartite matching)

**Algorithm**:

```
INPUT: SchedulingPeriodId
OUTPUT: Complete set of Assignments

1. Load all Activities for the period
2. Load all (Slot, Resource) pairs → Set L
3. For each Activity, compute excluded slots from constraints

4. Generate RANDOM PERMUTATION σ of L
   - Fisher-Yates shuffle
   - Assign rank 1 to |L| based on position
   - This randomization is the key to the competitive ratio

5. Process Activities sequentially:
   For each Activity a:
     a. Get excluded slots (hard constraints)
     b. Get user preferences (soft preferences)
     
     c. Filter valid candidates:
        - Remove slots in excluded set
        - Remove occupied (Slot, Resource) pairs
        - Check capacity constraints
     
     d. Calculate preference weights for each candidate
     
     e. Select using WEIGHTED RANDOM within top-ranked candidates:
        - Primary criterion: Earlier rank in permutation σ
        - Secondary criterion: Preference weight
        - Weight[i] = PreferenceWeight[i] * exp(-rank[i] * 0.1)
     
     f. Create Assignment, mark pair as occupied

6. Return results (created, unscheduled)
```

**Why This Works**:
- Random permutation prevents adversarial worst-case scenarios
- Greedy selection within permutation maintains competitive guarantee
- Preference weights enhance user satisfaction without breaking guarantees

**Complexity**: O(n × m) where n = Activities, m = (Slot, Resource) pairs

### 2. Online Mode: Greedy Re-matching

**Source**: Lecture 17, Section 2 (adapted for incremental changes)

**When Used**: New constraint added mid-semester

**Competitive Ratio**: ~0.5 (greedy bound, but prioritizes stability)

**Algorithm**:

```
INPUT: ActivityConstraintId (newly added)
OUTPUT: Minimal assignment modifications

1. Load the new ActivityConstraint
2. Get affected Activity
3. Get current Assignment for this Activity

4. Validate current assignment:
   - Recompute excluded slots (including new constraint)
   - If current Slot NOT in excluded set:
     ✓ No changes needed, return success
   - Else:
     ✗ Current assignment invalid, proceed to re-match

5. Re-match affected Activity:
   a. DELETE current Assignment (free the pair)
   
   b. Load all current Assignments
      - Build set of occupied pairs
   
   c. Find available pairs:
      - All pairs in SchedulingPeriod
      - MINUS occupied pairs
      - MINUS excluded slots
      - MINUS insufficient capacity
   
   d. If no valid pairs:
      - Mark Activity as unscheduled
      - Notify for manual intervention
      - Return partial failure
   
   e. Calculate preference weights
   
   f. Select using weighted random sampling
   
   g. Create new Assignment

6. Return result (1 modified, success/failure)
```

**Why This Approach**:
- **Stability**: Only touches directly affected Activities
- **Minimal Disruption**: Students/instructors already scheduled remain unchanged
- **Pragmatic**: Greedy is sufficient for incremental changes
- **Avoids Cascade**: Don't re-optimize entire schedule

**Complexity**: O(m) per affected Activity

## Component Details

### Constraint Processing

```
┌──────────────────────────┐
│ IConstraintProcessor     │
│ ┌──────────────────────┐ │
│ │ActivityConstraint    │ │
│ │Processor             │ │
│ └──────────┬───────────┘ │
│            │             │
│   ┌────────▼────────┐   │
│   │ Handler Registry│   │
│   └────────┬────────┘   │
│            │             │
│   ┌────────▼────────┐   │
│   │IConstraintHandler│  │
│   ├─────────────────┤   │
│   │ ExcludedWeekday │   │
│   │ RequiredCapacity│   │
│   │ TimeBlocking    │   │
│   │ ... (extensible)│   │
│   └─────────────────┘   │
└──────────────────────────┘
```

**Extensibility**: Add new constraint types by implementing `IConstraintHandler`

**Process**:
1. Fetch all ActivityConstraints for an Activity
2. For each constraint, invoke handler based on Key
3. Handler returns set of excluded Slot IDs
4. Union all excluded sets → final blacklist

### Preference Weighting

```
PreferenceWeightedRanker
├── CalculateWeight()
│   ├── Load UserPreferences
│   ├── Match preferences to candidate
│   └── Apply multipliers
│       ├── "preferred_time_morning" → 3.0x
│       ├── "preferred_weekday" → 3.0x
│       ├── "avoid_time_evening" → 0.3x
│       └── Base weight: 1.0x
└── SelectRandomWeighted()
    ├── Calculate cumulative weights
    ├── Generate random value in [0, totalWeight)
    └── Select candidate proportional to weight
```

**Weighted Random Sampling**:
- Higher weight → higher probability
- Maintains randomness for fairness
- Respects preferences without hard constraints

### Message Flow

```
Request Flow:
┌─────────┐   Publish    ┌──────────┐   Consume    ┌────────┐
│ MainApi │─────────────►│ RabbitMQ │─────────────►│ Engine │
└─────────┘   (async)    └──────────┘   (workers)  └───┬────┘
                                                        │
                                                    Execute
                                                        │
                                                        ▼
                                                   ┌────────┐
                                                   │  DB    │
                                                   └────────┘
Result Flow:
┌────────┐   Publish    ┌──────────┐   Consume    ┌─────────┐
│ Engine │─────────────►│ RabbitMQ │─────────────►│ MainApi │
└────────┘              └──────────┘   (optional) └─────────┘
```

**Queue Configuration**:
- `chronos.scheduling.batch` - Batch scheduling requests
- `chronos.scheduling.online` - Online scheduling requests
- Durable queues, manual acknowledgments
- Dead-letter queues for error handling

## Data Model

### Domain Entities

```csharp
// Left side (L) of bipartite graph
Slot {
    Id, OrganizationId, SchedulingPeriodId
    Weekday, FromTime, ToTime
}

Resource {
    Id, OrganizationId, ResourceTypeId
    Location, Identifier, Capacity?
}

// Right side (R) of bipartite graph
Activity {
    Id, OrganizationId, SubjectId
    AssignedUserId, ActivityType, ExpectedStudents?
}

// Matching (edges)
Assignment {
    Id, OrganizationId
    SlotId, ResourceId, ScheduledItemId (= ActivityId)
    // Unique constraint: (SlotId, ResourceId)
}

// Constraints
ActivityConstraint {
    Id, OrganizationId, ActivityId
    Key, Value  // e.g., "excluded_weekdays", "Monday,Wednesday"
}

// Preferences
UserPreference {
    Id, OrganizationId, UserId, SchedulingPeriodId
    Key, Value  // e.g., "preferred_time_morning", "true"
}
```

### Messages

```csharp
SchedulePeriodRequest(
    SchedulingPeriodId, OrganizationId, Mode
)

HandleConstraintChangeRequest(
    ActivityConstraintId, OrganizationId, SchedulingPeriodId, Mode
)

SchedulingResult(
    RequestId, Success,
    AssignmentsCreated, AssignmentsModified,
    UnscheduledActivityIds, FailureReason?
)
```

## Deployment

### Local Development
```powershell
# Ensure RabbitMQ service is running
Get-Service RabbitMQ

# Run the engine
cd src/Chronos.Engine
dotnet run
```

### Production
```powershell
# Use managed RabbitMQ service (CloudAMQP, Amazon MQ)
# Configure connection string in environment variables
dotnet publish -c Release
# Deploy to hosting environment (Azure App Service, AWS, etc.)
```

### Scaling
- **Horizontal**: Run multiple Engine instances (RabbitMQ load-balances consumers)
- **Vertical**: Increase worker memory for large scheduling periods
- **Queue Partitioning**: Separate queues per organization for isolation
- **Windows Service**: Deploy as Windows Service for always-on operation

## Performance Characteristics

### Batch Mode (Ranking Algorithm)
- **Time**: O(n × m) - n Activities, m (Slot, Resource) pairs
- **Space**: O(m) - Store permutation and occupied set
- **Throughput**: ~100-1000 Activities/second (depends on m)

### Online Mode
- **Time**: O(m) per affected Activity
- **Space**: O(m) - Store occupied pairs
- **Throughput**: ~1000+ constraint changes/second

### Bottlenecks
1. **Database I/O**: Loading slots, resources, assignments
   - Mitigation: Caching, read replicas
2. **Constraint Processing**: Complex constraint logic
   - Mitigation: Optimize handlers, index Slot IDs
3. **Message Queue**: High request volume
   - Mitigation: Multiple consumers, partitioning

## Testing Strategy

### Unit Tests
- Constraint processor with various constraint types
- Preference-weighted ranker with different distributions
- Matching algorithms with mock data

### Integration Tests
- End-to-end message flow with test RabbitMQ
- Database integration with test data
- Verify Assignment creation and modification

### Load Tests
- Process 1000+ Activities in batch mode
- Simulate concurrent online updates
- Measure throughput and latency

### Acceptance Tests
- Scenario: Full semester scheduling
- Scenario: Mid-semester constraint addition
- Scenario: Conflicting constraints (should fail gracefully)

## Monitoring & Observability

### Key Metrics
- **Queue Depth**: Should stay near 0 (backlog indicator)
- **Processing Time**: P50, P95, P99 latency per Activity
- **Success Rate**: % of Activities successfully scheduled
- **Constraint Violations**: Should be 0 (hard constraints)
- **Preference Satisfaction**: Average weight of selected candidates

### Logging
- Structured logging with correlation IDs
- Log levels: Debug (algorithm steps), Info (results), Error (failures)
- Key events: Request received, matching started, assignment created

### Alerts
- Queue depth > 100 for > 5 minutes
- Success rate < 90%
- Processing time > 10 seconds
- RabbitMQ connection failures

## Security Considerations

- **Multi-tenancy**: OrganizationId filtering in database queries
- **Message Validation**: Deserialize and validate all requests
- **RabbitMQ Authentication**: Use strong credentials in production
- **Rate Limiting**: Prevent abuse via queue depth monitoring

## Future Enhancements

1. **Optimal Offline Matching**: Replace Ranking with Hungarian algorithm when all data known
2. **Machine Learning**: Learn preference weights from historical satisfaction scores
3. **Constraint Solver Integration**: Use CP-SAT for complex constraint problems
4. **Multi-objective Optimization**: Balance multiple criteria (cost, satisfaction, fairness)
5. **What-If Analysis**: Simulate schedules before committing
6. **Rollback Mechanism**: Snapshot schedules for easy restoration

## References

- **Lecture 17**: Online Algorithms - Matchings (Ranking Algorithm)
- **Competitive Analysis**: [Karp, Vazirani, Vazirani 1990]
- **RabbitMQ Documentation**: https://www.rabbitmq.com/documentation.html
- **Entity Framework Core**: https://learn.microsoft.com/en-us/ef/core/

