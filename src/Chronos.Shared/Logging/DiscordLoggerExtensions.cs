using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chronos.Shared.Logging;

/// <summary>
/// Extension methods for adding Discord logger to the logging system
/// </summary>
public static class DiscordLoggerExtensions
{
    /// <summary>
    /// Adds Discord logger to the logging builder with configuration from appsettings.json
    /// </summary>
    /// <param name="builder">The logging builder</param>
    /// <param name="botName">The name to display for the bot in Discord (e.g., "MainApiBguVM")</param>
    public static ILoggingBuilder AddDiscordLogger(
        this ILoggingBuilder builder,
        string botName)
    {
        builder.Services.Configure<DiscordLoggerConfiguration>(config =>
        {
            config.BotName = botName;
        });
        builder.Services.AddSingleton<ILoggerProvider, DiscordLoggerProvider>();
        return builder;
    }

    /// <summary>
    /// Adds Discord logger to the logging builder with manual configuration
    /// </summary>
    /// <param name="builder">The logging builder</param>
    /// <param name="botName">The name to display for the bot in Discord (e.g., "MainApiBguVM")</param>
    /// <param name="configure">Configuration action</param>
    public static ILoggingBuilder AddDiscordLogger(
        this ILoggingBuilder builder,
        string botName,
        Action<DiscordLoggerConfiguration> configure)
    {
        builder.Services.Configure<DiscordLoggerConfiguration>(config =>
        {
            configure(config);
            config.BotName = botName; // Override with the provided bot name
        });
        builder.Services.AddSingleton<ILoggerProvider, DiscordLoggerProvider>();
        return builder;
    }
}
