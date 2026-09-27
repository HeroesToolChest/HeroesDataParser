using CASCLib;

namespace HeroesDataParser;

public class CASCLoggerOptions : ILoggerOptions
{
    public string LogFileName => Path.Combine(SerilogLogging.LogDirectoryPath, "casclib.log");

    public bool TimeStamp => true;
}
