# Quick Start Guide - Chronos Scheduling Engine

## Overview

This guide will help you get the Chronos Scheduling Engine up and running in under 10 minutes.

## Prerequisites

- ✅ .NET 8.0 SDK installed
- ✅ RabbitMQ installed and running
- ✅ Git (to clone the repository)

## Step 1: Install and Start RabbitMQ

**Option A: Windows Installer**
1. Download RabbitMQ: https://www.rabbitmq.com/install-windows.html
2. Install and start the service
3. Enable management plugin:
```powershell
rabbitmq-plugins enable rabbitmq_management
```

**Option B: Chocolatey**
```powershell
choco install rabbitmq
rabbitmq-plugins enable rabbitmq_management
```

**Verify RabbitMQ is running**:
- Check Windows Services for "RabbitMQ" service (should be Running)
- Or check if port 5672 is listening

**Access RabbitMQ Management UI**:
- URL: http://localhost:15672
- Username: `guest`
- Password: `guest`

## Step 2: Run the Scheduling Engine

Open PowerShell/Terminal and navigate to the Engine directory:

```powershell
cd src/Chronos.Engine
dotnet run
```

**Expected Output**:
```
info: Chronos.Engine.Messaging.RabbitMqConnectionFactory[0]
      Creating RabbitMQ connection to localhost:5672
info: Chronos.Engine.Messaging.RabbitMqConnectionFactory[0]
      RabbitMQ connection established successfully
info: Chronos.Engine.Messaging.BatchSchedulingConsumer[0]
      Batch Scheduling Consumer started, listening to queue: chronos.scheduling.batch
info: Chronos.Engine.Messaging.OnlineSchedulingConsumer[0]
      Online Scheduling Consumer started, listening to queue: chronos.scheduling.online
```

## Step 3: Verify Queues Created

Go back to RabbitMQ Management UI (http://localhost:15672) and click on the **Queues** tab.

You should see two queues:
- ✅ `chronos.scheduling.batch`
- ✅ `chronos.scheduling.online`

Both should show "Ready: 0" and "Consumers: 1".

## Step 4: Test the System

### Option A: Using RabbitMQ Management UI

1. **Go to Queues tab** in RabbitMQ UI
2. **Click on** `chronos.scheduling.batch`
3. **Expand "Publish message"** section
4. **Set Payload**:
```json
{
  "schedulingPeriodId": "11111111-1111-1111-1111-111111111111",
  "organizationId": "22222222-2222-2222-2222-222222222222",
  "mode": 0
}
```
5. **Click "Publish message"**
6. **Check Engine logs** - You should see:
```
info: Starting Ranking Algorithm for SchedulingPeriod 11111111-1111-1111-1111-111111111111
```

### Option B: Using Code (C#)

Create a simple console app to publish messages:

```csharp
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

var factory = new ConnectionFactory
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Batch Scheduling Request
var request = new
{
    schedulingPeriodId = Guid.NewGuid(),
    organizationId = Guid.NewGuid(),
    mode = 0 // Batch
};

var json = JsonSerializer.Serialize(request);
var body = Encoding.UTF8.GetBytes(json);

channel.BasicPublish(
    exchange: "chronos.scheduling",
    routingKey: "request.batch",
    basicProperties: null,
    body: body);

Console.WriteLine("Published batch scheduling request!");
```

## Understanding the Flow

### Batch Scheduling (Before Semester)

```
1. MainApi publishes SchedulePeriodRequest
   ↓
2. RabbitMQ routes to batch queue
   ↓
3. BatchSchedulingConsumer picks up message
   ↓
4. RankingAlgorithmStrategy executes:
   - Generates random permutation
   - Processes all Activities
   - Creates Assignments
   ↓
5. Result published back to RabbitMQ
```

### Online Scheduling (Mid-Semester)

```
1. User adds new constraint in UI
   ↓
2. MainApi saves ActivityConstraint to database
   ↓
3. MainApi publishes HandleConstraintChangeRequest
   ↓
4. RabbitMQ routes to online queue
   ↓
5. OnlineSchedulingConsumer picks up message
   ↓
6. OnlineMatchingStrategy executes:
   - Checks if current assignment still valid
   - If invalid, re-matches Activity
   - Minimal changes
   ↓
7. Result published back to RabbitMQ
```

## Troubleshooting

### Engine won't start

**Error**: `Unable to connect to RabbitMQ`

**Solution**:
```powershell
# Check if RabbitMQ service is running
Get-Service RabbitMQ

# If not running, start it
Start-Service RabbitMQ

# Or check from Services management console (services.msc)
# Then try running the Engine again
```

### No messages being processed

**Check**:
1. Are queues visible in RabbitMQ UI?
2. Do queues show "Consumers: 1"?
3. Check Engine logs for errors

**Common Issue**: Message format incorrect
- Ensure JSON matches the request schema
- Check property names (camelCase)
- Verify GUIDs are valid format

### "Activity not found" errors

**Cause**: InMemoryDatabase is empty by default

**Solution**: You need to either:
1. Seed the database with test data
2. Or publish requests after creating Activities via MainApi

## Next Steps

### 1. Explore the Code

Key files to review:
- `Matching/RankingAlgorithmStrategy.cs` - Batch algorithm
- `Matching/OnlineMatchingStrategy.cs` - Online algorithm
- `Constraints/ActivityConstraintProcessor.cs` - Constraint handling

### 2. Add Sample Data

Create a script to seed the InMemoryDatabase with:
- Organizations
- SchedulingPeriods
- Slots (time blocks)
- Resources (classrooms)
- Activities (classes to schedule)

### 3. Customize Constraint Handlers

Implement your own constraint logic:

```csharp
public class MyCustomConstraintHandler : IConstraintHandler
{
    public string ConstraintKey => "my_custom_constraint";
    
    public async Task<HashSet<Guid>> ProcessConstraintAsync(
        ActivityConstraint constraint,
        Guid organizationId)
    {
        // Your logic here
        return new HashSet<Guid>();
    }
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddScoped<IConstraintHandler, MyCustomConstraintHandler>();
```

### 4. Monitor Performance

Watch these metrics:
- Queue depth (should stay near 0)
- Processing time per Activity
- Success rate of assignments
- Memory usage

### 5. Production Deployment

For production:
1. Replace InMemoryDatabase with PostgreSQL/SQL Server
2. Use managed RabbitMQ (CloudAMQP, Amazon MQ)
3. Add Application Insights or Prometheus
4. Deploy using Docker containers
5. Set up health checks and alerts

## Useful Commands

```powershell
# View Engine logs in real-time
cd src/Chronos.Engine
dotnet run

# Stop RabbitMQ service
Stop-Service RabbitMQ

# Start RabbitMQ service
Start-Service RabbitMQ

# Restart RabbitMQ service
Restart-Service RabbitMQ

# Check RabbitMQ service status
Get-Service RabbitMQ

# Build the Engine
dotnet build

# Publish for deployment
dotnet publish -c Release
```

## Resources

- **README**: `src/Chronos.Engine/README.md` - Detailed documentation
- **Architecture**: `ARCHITECTURE.md` - System architecture
- **Implementation**: `IMPLEMENTATION_SUMMARY.md` - What was built
- **RabbitMQ Docs**: https://www.rabbitmq.com/documentation.html
- **Lecture Notes**: See `lec17.pdf` for algorithm theory

## Getting Help

If you encounter issues:
1. Check the logs (Engine console output)
2. Review RabbitMQ Management UI for queue status
3. Verify configuration in `appsettings.json`
4. Check that RabbitMQ service is running (`Get-Service RabbitMQ`)

## Success!

If you can:
- ✅ See Engine running without errors
- ✅ See queues in RabbitMQ UI
- ✅ Publish a test message
- ✅ See Engine process the message

Then you're all set! The scheduling engine is working correctly.

Next, integrate with MainApi to trigger scheduling from the REST API.

