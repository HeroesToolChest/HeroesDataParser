using CASCLib;

namespace HeroesDataParser;

public class CASCLoggerOptions : ILoggerOptions
{
    public string LogFileName => Path.Combine(AppContext.BaseDirectory, SerilogLogging.LogDirectory, "casclib.log");

    public bool TimeStamp => true;
}
