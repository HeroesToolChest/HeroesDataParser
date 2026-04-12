namespace HeroesDataParser.Core;

public interface IXmlMapDataParserProcessor
{
    Task ExecuteMapParser();

    Task ExecuteJsonDataFileWriteTask();
}
