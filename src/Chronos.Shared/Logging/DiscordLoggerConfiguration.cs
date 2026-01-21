using Microsoft.Extensions.Logging;

namespace Chronos.Shared.Logging;

/// <summary>
/// Configuration for Discord logger
/// </summary>
public class DiscordLoggerConfiguration
{
    /// <summary>
    /// Discord webhook URL. If not provided, logs will only be written to console.
    /// </summary>
    public string? DiscordWebhookUrl { get; set; }

    /// <summary>
    /// Minimum log level to capture. Default is Information.
    /// </summary>
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// How often to flush logs to Discord (in seconds). Default is 30 seconds.
    /// </summary>
    public int FlushIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// Bot name to display in Discord. Default is "Chronos Logger".
    /// </summary>
    public string BotName { get; set; } = "Chronos Logger";
}
