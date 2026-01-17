# Chronos.Engine - Scheduling Engine

Asynchronous scheduling engine implementing dual-mode bipartite matching algorithms for Activity-to-(Slot, Resource) assignment.

## Architecture Overview

The engine implements two distinct scheduling strategies based on the online bipartite matching algorithm from Lecture 17:

### 1. Batch Mode (Ranking Algorithm)
- **Use Case**: Schedule entire SchedulingPeriod before semester starts
- **Algorithm**: Ranking Algorithm (random permutation)
- **Competitive Ratio**: 1-1/e ≈ 0.632
- **Input**: All Activities for a scheduling period
- **Output**: Complete assignment schedule

### 2. Online Mode (Greedy Matching)
- **Use Case**: Handle mid-semester constraint additions
- **Algorithm**: Greedy re-matching with minimal disruption
- **Competitive Ratio**: ~0.5 (greedy bound)
- **Input**: New ActivityConstraint
- **Output**: Minimal assignment modifications

## Key Components

### Matching Strategies
- **RankingAlgorithmStrategy**: Batch mode using random permutation for optimal competitive ratio
- **OnlineMatchingStrategy**: Online mode for incremental updates with stability preservation
- **MatchingOrchestrator**: Routes requests to appropriate strategy

### Constraint Processing
- **IConstraintProcessor**: Determines excluded Slot IDs based on constraints
- **ActivityConstraintProcessor**: Extensible processor using strategy pattern
- **IConstraintHandler**: Plugin interface for custom constraint types

### Messaging Infrastructure
- **RabbitMQ**: Message queue for asynchronous request processing
- **BatchSchedulingConsumer**: Processes batch scheduling requests
- **OnlineSchedulingConsumer**: Processes online scheduling requests
- **MessagePublisher**: Publishes scheduling results

### Preference Weighting
- **PreferenceWeightedRanker**: Calculates weights and performs weighted random selection
- Supports soft preferences (e.g., "prefer mornings" → 3x weight)
- Hard constraints are strictly enforced through filtering

## Setup

### Prerequisites
- .NET 8.0 SDK
- RabbitMQ server (local or cloud)

### Local Development Setup

1. **Install RabbitMQ**:

**Option A: Windows Installer**
- Download from: https://www.rabbitmq.com/install-windows.html
- Install and start RabbitMQ service
- Enable management plugin: `rabbitmq-plugins enable rabbitmq_management`

**Option B: Chocolatey**
```powershell
choco install rabbitmq
rabbitmq-plugins enable rabbitmq_management
```

Access management UI: http://localhost:15672 (guest/guest)

2. **Configure Connection** (appsettings.Development.json):
```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "BatchQueueName": "chronos.scheduling.batch",
    "OnlineQueueName": "chronos.scheduling.online",
    "ExchangeName": "chronos.scheduling"
  }
}
```

3. **Run the Engine**:
```bash
cd chronos-service/src/Chronos.Engine
dotnet run
```

### Production Setup

For production, use a managed RabbitMQ service:
- **CloudAMQP**: https://www.cloudamqp.com/ (free tier available)
- **Amazon MQ**: https://aws.amazon.com/amazon-mq/
- **Azure Service Bus**: Alternative to RabbitMQ

Update configuration via environment variables:
```powershell
$env:RabbitMQ__HostName="your-rabbitmq-host"
$env:RabbitMQ__Port="5672"
$env:RabbitMQ__UserName="your-username"
$env:RabbitMQ__Password="your-password"
```

## Usage

### Batch Scheduling (Pre-Semester)

Publish a `SchedulePeriodRequest` to the batch queue:

```csharp
var request = new SchedulePeriodRequest(
    SchedulingPeriodId: periodId,
    OrganizationId: orgId,
    Mode: SchedulingMode.Batch
);

await messagePublisher.PublishAsync(request, "request.batch");
```

The engine will:
1. Load all Activities for the period
2. Load all (Slot, Resource) pairs
3. Generate random permutation (the "ranking")
4. Match Activities sequentially using preference-weighted selection
5. Publish `SchedulingResult` with success/failure status

### Online Scheduling (Mid-Semester)

Publish a `HandleConstraintChangeRequest` when a new constraint is added:

```csharp
var request = new HandleConstraintChangeRequest(
    ActivityConstraintId: constraintId,
    OrganizationId: orgId,
    SchedulingPeriodId: periodId,
    Mode: SchedulingMode.Online
);

await messagePublisher.PublishAsync(request, "request.online");
```

The engine will:
1. Load the new constraint
2. Check if affected Activity's current assignment is still valid
3. If invalid, delete and re-match using greedy algorithm
4. Publish `SchedulingResult` with modification details

## Adding Custom Constraint Handlers

Create a new handler implementing `IConstraintHandler`:

```csharp
public class MyConstraintHandler : IConstraintHandler
{
    public string ConstraintKey => "my_constraint_type";
    
    private readonly ISlotRepository _slotRepository;
    
    public MyConstraintHandler(ISlotRepository slotRepository)
    {
        _slotRepository = slotRepository;
    }
    
    public async Task<HashSet<Guid>> ProcessConstraintAsync(
        ActivityConstraint constraint,
        Guid organizationId)
    {
        // Parse constraint.Value
        // Determine which Slot IDs should be excluded
        // Return HashSet of excluded Slot IDs
        
        return new HashSet<Guid>();
    }
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddScoped<IConstraintHandler, MyConstraintHandler>();
```

## Constraint Evaluation

The engine provides a flexible constraint evaluation system to determine whether an Activity can be assigned to a specific (Slot, Resource) pair.

### Using IConstraintEvaluator

```csharp
public class SchedulingService
{
    private readonly IConstraintEvaluator _evaluator;
    
    public async Task<bool> ValidateAssignmentAsync(
        Activity activity, 
        Slot slot, 
        Resource resource)
    {
        // Quick validation - returns true if no hard constraint violations
        var canAssign = await _evaluator.CanAssignAsync(activity, slot, resource);
        
        if (!canAssign)
        {
            // Get detailed violations for logging/reporting
            var violations = await _evaluator.GetViolationsAsync(activity, slot, resource);
            
            foreach (var violation in violations)
            {
                _logger.LogWarning(
                    "Constraint violation: {Key} - {Message} (Severity: {Severity})",
                    violation.ConstraintKey,
                    violation.Message,
                    violation.Severity);
            }
        }
        
        return canAssign;
    }
}
```

### Supported Constraint Types

The engine supports 5 built-in constraint types:

1. **preferred_weekdays** (Soft) - Preferred weekdays for the activity
2. **time_range** (Hard) - Required time range for the slot
3. **required_capacity** (Hard) - Minimum/maximum resource capacity
4. **location_preference** (Soft) - Preferred resource locations
5. **compatible_resource_types** (Hard) - Compatible resource types for the activity

For detailed constraint format documentation, see [docs/CONSTRAINT_FORMATS.md](docs/CONSTRAINT_FORMATS.md).

### Performance

All constraint evaluations are designed to complete in **<1ms**:
- Single constraint: ~0.15ms
- Five constraints: ~0.65ms
- Suitable for real-time validation during scheduling

### Adding Custom Validators

Create a validator implementing `IConstraintValidator`:

```csharp
public class MyConstraintValidator : IConstraintValidator
{
    public string ConstraintKey => "my_constraint_type";
    
    public async Task<ConstraintViolation?> ValidateAsync(
        ActivityConstraint constraint,
        Activity activity,
        Slot slot,
        Resource resource)
    {
        // Validation logic
        // Return ConstraintViolation if violated, null if satisfied
        
        return null;
    }
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddScoped<IConstraintValidator, MyConstraintValidator>();
```

## Monitoring

### RabbitMQ Management UI
- URL: http://localhost:15672
- View queues, message rates, consumer status
- Monitor message throughput and errors

### Logs
The engine uses structured logging. Key events:
- `"Starting Ranking Algorithm"` - Batch scheduling initiated
- `"Starting Online Matching"` - Online update initiated
- `"Matched Activity X to Y"` - Successful assignment
- `"No valid candidates for Activity X"` - Scheduling failure

### Metrics to Monitor
- Queue depth (should stay near 0)
- Consumer throughput (messages/second)
- Scheduling success rate
- Average processing time per Activity

## Theoretical Foundation

Based on **Lecture 17: Online Algorithms - Matchings**

### Why Ranking Algorithm?
- **Deterministic algorithms**: Limited to 1/2 competitive ratio
- **Random neighbor selection**: Approaches 1/2 in worst case
- **Ranking Algorithm**: Achieves **(1-1/e) ≈ 0.632** competitive ratio
- Key insight: Pre-randomize order of L to prevent adversarial exploitation

### Algorithm Properties
- **Randomized permutation** breaks deterministic barriers
- **Greedy within permutation** ensures competitive guarantee
- **Preference weights** enhance user satisfaction without breaking guarantees
- **Hard constraints** maintain feasibility (strict filtering)

## Troubleshooting

### Engine not consuming messages
- Check RabbitMQ is running (Services in Windows or check port 5672)
- Verify queue configuration in appsettings.json
- Check logs for connection errors
- Ensure RabbitMQ management plugin is enabled

### Assignments failing to create
- Verify database connection (InMemoryDatabase by default)
- Check constraint logic (may be over-constrained)
- Review logs for excluded slot counts

### Performance issues
- Monitor queue depth - if growing, add more engine instances
- Check database query performance
- Consider caching frequently-accessed data

## Development

### Running Tests
```powershell
cd chronos-service/tests/Chronos.Tests.Engine
dotnet test
```

### Building
```powershell
cd chronos-service/src/Chronos.Engine
dotnet build
```

### Publishing
```powershell
dotnet publish -c Release -o ./publish
```

## Future Enhancements

- **Adaptive Adversary Defense**: Handle worst-case input orderings
- **Machine Learning**: Learn preference weights from historical data
- **Parallel Processing**: Multiple engine workers for high throughput
- **Constraint Conflict Detection**: Pre-validate before applying
- **Rollback Mechanism**: Save schedule snapshots for easy rollback

