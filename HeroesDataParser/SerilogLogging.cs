using Serilog;
using Serilog.Configuration;

namespace HeroesDataParser;

internal static class SerilogLogging
{
    public const string LogDirectory = "logs";
    public const string LogPrefix = "log";
    public const int RetainedFileCountLimit = 7;

    public static DateTime StartDateTime { get; } = DateTime.Now;

    public static Action<LoggerSinkConfiguration> LoggerConfigure()
    {
        return x => x.File(new CompactJsonFormatter(), Path.Combine(AppContext.BaseDirectory, LogDirectory, $"{LogPrefix}{StartDateTime:yyyyMMdd_HHmmss}.txt"), retainedFileCountLimit: RetainedFileCountLimit, fileSizeLimitBytes: 1024 * 1024 * 64);
    }
}
