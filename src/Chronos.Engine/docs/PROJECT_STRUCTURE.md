# Chronos.Engine - Project Structure

## Overview

All scheduling engine code, configuration, and documentation is contained within the `Chronos.Engine` directory.

## Directory Structure

```
Chronos.Engine/
├── AppSettings/            # Configuration files
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── Configuration/           # RabbitMQ and other configuration classes
│   └── RabbitMqOptions.cs
│
├── Constraints/            # Constraint processing system
│   ├── IConstraintProcessor.cs
│   ├── IConstraintHandler.cs
│   ├── ActivityConstraintProcessor.cs
│   └── Handlers/
│       └── ExampleExcludedWeekdayConstraintHandler.cs
│
├── Matching/              # Matching algorithms (Ranking & Online)
│   ├── IMatchingStrategy.cs
│   ├── SlotResourcePair.cs
│   ├── PreferenceWeightedRanker.cs
│   ├── RankingAlgorithmStrategy.cs      # Batch mode (1-1/e competitive)
│   ├── OnlineMatchingStrategy.cs        # Online mode (greedy)
│   └── MatchingOrchestrator.cs
│
├── Messaging/             # RabbitMQ infrastructure
│   ├── IRabbitMqConnectionFactory.cs
│   ├── RabbitMqConnectionFactory.cs
│   ├── IMessagePublisher.cs
│   ├── MessagePublisher.cs
│   ├── BatchSchedulingConsumer.cs
│   └── OnlineSchedulingConsumer.cs
│
├── docs/                  # Documentation
│   ├── ARCHITECTURE.md              # Complete system architecture
│   ├── IMPLEMENTATION_SUMMARY.md    # What was built
│   ├── QUICKSTART_GUIDE.md          # Get started in 10 minutes
│   └── PROJECT_STRUCTURE.md         # This file
│
├── Properties/
│   └── launchSettings.json
│
├── Program.cs             # Application entry point & DI configuration
├── Chronos.Engine.csproj  # Project file with dependencies
└── README.md             # Main documentation & getting started
```

## External Dependencies

### Domain Messages
Located in: `Chronos.Domain/Schedule/Messages/`
- `SchedulingMode.cs` - Enum for Batch/Online modes
- `SchedulePeriodRequest.cs` - Batch scheduling request
- `HandleConstraintChangeRequest.cs` - Online scheduling request
- `SchedulingResult.cs` - Result message

These are shared between MainApi and Engine for message contracts.

## Documentation Guide

### For Getting Started
→ Start with **README.md** in the root of Chronos.Engine

### For Understanding the System
→ Read **docs/ARCHITECTURE.md** for complete technical details

### For Quick Setup
→ Follow **docs/QUICKSTART_GUIDE.md** for 10-minute setup

### For Implementation Details
→ See **docs/IMPLEMENTATION_SUMMARY.md** for what was built

### For Project Organization
→ This file explains the directory structure

## Key Files

### Entry Point
- **Program.cs** - Sets up dependency injection and starts the engine

### Core Algorithms
- **Matching/RankingAlgorithmStrategy.cs** - Batch mode scheduling (1-1/e competitive)
- **Matching/OnlineMatchingStrategy.cs** - Online mode re-matching (minimal disruption)

### Infrastructure
- **Messaging/BatchSchedulingConsumer.cs** - Processes batch requests from queue
- **Messaging/OnlineSchedulingConsumer.cs** - Processes online requests from queue

### Configuration
- **AppSettings/appsettings.json** - RabbitMQ connection, queue names
- **AppSettings/appsettings.Development.json** - Development overrides

## Running the Engine

### Development
```powershell
# Ensure RabbitMQ is running
Get-Service RabbitMQ

# From Chronos.Engine directory
dotnet run
```

### Production
```powershell
# From Chronos.Engine directory
dotnet publish -c Release
# Deploy to hosting environment
```

## Adding Features

### New Constraint Handler
1. Create handler in `Constraints/Handlers/`
2. Implement `IConstraintHandler`
3. Register in `Program.cs`

### New Matching Strategy
1. Create strategy in `Matching/`
2. Implement `IMatchingStrategy`
3. Register in `Program.cs`

## Testing

Tests will be in: `chronos-service/tests/Chronos.Tests.Engine/` (to be created)

## Related Projects

- **Chronos.Domain** - Domain entities and messages
- **Chronos.Data** - Database repositories
- **Chronos.MainApi** - REST API (triggers scheduling)

## Changes from Original Structure

The following files were **NOT** modified outside Chronos.Engine:
- ✅ `Chronos.Data/DataDependencyInjectionExtension.cs` - Reverted to original
- ✅ All other projects remain unchanged

All scheduling engine functionality is self-contained within `Chronos.Engine/`.

