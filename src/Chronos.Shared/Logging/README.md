# Discord Reporting Logger

A reusable Discord logging system for reporting logs to Discord channels via webhooks. This logger batches logs and sends them periodically to Discord, with automatic fallback to console-only logging if no webhook is configured.

## Features

- **Batch Logging**: Collects logs and sends them in batches to reduce API calls
- **Console Fallback**: If no Discord webhook is provided, logs are written to console only
- **Message Splitting**: Automatically splits large log batches into multiple Discord messages (respects ~2000 character limit)
- **Configurable**: Set minimum log level, flush interval, and bot name
- **Color-coded Console**: Different log levels have different colors in console output
- **Thread-safe**: Uses concurrent collections for safe multi-threaded logging
- **Exception Handling**: Captures and formats exceptions with stack traces

## Configuration

### Using Environment Variables (Recommended for Production)

For Docker or production deployments, use environment variables:

Add to your [`.local.env`](.local.env) file:

```bash
DiscordLogger__DiscordWebhookUrl=https://discord.com/api/webhooks/YOUR_WEBHOOK_ID/YOUR_WEBHOOK_TOKEN
```

The [`docker-compose.yml`](docker-compose.yml) loads this file automatically via `env_file: .local.env`, and ASP.NET Core will automatically bind the environment variable to the configuration using the double underscore (`__`) separator.

### Using appsettings.json (Local Development)

Add the following section to your `appsettings.json` (or `appsettings.Development.json`):

```json
{
    "DiscordLogger": {
        "DiscordWebhookUrl": "",
        "MinimumLogLevel": "Information",
        "FlushIntervalSeconds": 30
    }
}
```

**Note**: Environment variables will override appsettings.json values. If `DiscordLogger__DiscordWebhookUrl` is set in the environment, it will take precedence.

### Configuration Options

- **DiscordWebhookUrl**: Discord webhook URL. Leave empty for console-only logging.
- **MinimumLogLevel**: Minimum log level to capture. Options: `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`. Default: `Information`.
- **FlushIntervalSeconds**: How often to flush logs to Discord (in seconds). Default: 30.

**Note**: The bot name is passed as a parameter when calling `AddDiscordLogger()` (see Usage section below).

## Usage

### Setup in Program.cs

```csharp
using Chronos.Shared.Logging;

var builder = WebApplication.CreateBuilder(args);

// Option 1: Configure from appsettings.json (recommended)
builder.Services.Configure<DiscordLoggerConfiguration>(
    builder.Configuration.GetSection("DiscordLogger"));
builder.Logging.AddDiscordLogger("MainApiBguVM"); // Pass your service name

// Option 2: Configure programmatically
builder.Logging.AddDiscordLogger("MyServiceName", config =>
{
    config.DiscordWebhookUrl = "https://discord.com/api/webhooks/YOUR_WEBHOOK_ID/YOUR_WEBHOOK_TOKEN";
    config.MinimumLogLevel = LogLevel.Information;
    config.FlushIntervalSeconds = 30;
});

var app = builder.Build();
// ... rest of your application setup
```

### Using the Logger

The Discord logger integrates with the standard `ILogger<T>` interface:

```csharp
public class MyService
{
    private readonly ILogger<MyService> _logger;

    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }

    public void DoSomething()
    {
        _logger.LogInformation("Starting operation");

        try
        {
            // Your code here
            _logger.LogDebug("Debug information");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed");
        }

        _logger.LogInformation("Operation completed");
    }
}
```

## How It Works

1. **Log Collection**: Logs are added to a concurrent queue as they are logged
2. **Console Output**: Each log is immediately written to console with color coding
3. **Periodic Flushing**: A timer flushes the log queue every `FlushIntervalSeconds`
4. **Batching**: All queued logs are combined into one or more Discord messages
5. **Message Splitting**: If logs exceed ~1900 characters, they're split into multiple messages
6. **Discord Sending**: Messages are sent to Discord via webhook with rate limiting
7. **Fallback**: If webhook fails or isn't configured, console logging continues

## Discord Webhook Setup

1. Go to your Discord server
2. Navigate to Server Settings → Integrations → Webhooks
3. Click "New Webhook"
4. Name your webhook (e.g., "Chronos Logger")
5. Select the channel where logs should be sent
6. Copy the webhook URL
7. Add the URL to your configuration

## Message Format

Logs are formatted with emojis for easy visual scanning:

- 🔍 Trace
- 🐛 Debug
- ℹ️ Information
- ⚠️ Warning
- ❌ Error
- 🔥 Critical

Example Discord message:

```
ℹ️ [2026-01-21 16:00:00] [Information] MyService
   Starting operation

🐛 [2026-01-21 16:00:01] [Debug] MyService
   Processing item 1

❌ [2026-01-21 16:00:02] [Error] MyService
   Operation failed
   Exception: InvalidOperationException: Something went wrong
   at MyService.DoSomething() in MyService.cs:line 42
```

## Best Practices

1. **Set Appropriate Log Level**: Use `Information` or higher in production to avoid spam
2. **Configure Flush Interval**: Balance between real-time logging and API rate limits (30-60 seconds recommended)
3. **Monitor Discord Rate Limits**: The logger respects Discord's rate limits (5 requests per 2 seconds)
4. **Test Without Webhook**: Test your application without a webhook URL to ensure console fallback works
5. **Secure Your Webhook**: Keep your webhook URL secret; consider using environment variables

## Rate Limiting

Discord webhooks have the following rate limits:

- 5 requests per 2 seconds per webhook
- 2000 character limit per message

The logger automatically:

- Adds 500ms delay between messages
- Splits messages that exceed ~1900 characters
- Handles rate limit responses gracefully

## Dependencies

- `Microsoft.Extensions.Logging`
- `Microsoft.Extensions.Options`
- `System.Net.Http.Json`

These are already included in most ASP.NET Core projects.
