using CASCLib;

namespace HeroesDataParser;

public class CASCLoggerOptions : ILoggerOptions
{
    public string LogFileName => Path.Join(SerilogLogging.LogDirectory, "casclib.log");

    public bool TimeStamp => true;
}
