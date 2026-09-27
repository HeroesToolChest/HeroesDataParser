using Serilog;
using Serilog.Configuration;

namespace HeroesDataParser;

internal static class SerilogLogging
{
    public const string LogDirectoryName = "logs";
    public const string LogPrefixName = "log";
    public const int RetainedFileCountLimit = 7;

    private static readonly DateTime _startDateTime = DateTime.Now;

    public static string LogDirectoryPath => AppPaths.GetDirectory(LogDirectoryName);

    public static Action<LoggerSinkConfiguration> LoggerConfigure()
    {
        return x => x.File(new CompactJsonFormatter(), Path.Combine(LogDirectoryPath, $"{LogPrefixName}{_startDateTime:yyyyMMdd_HHmmss}.txt"), retainedFileCountLimit: RetainedFileCountLimit, fileSizeLimitBytes: 1024 * 1024 * 64);
    }
}
