namespace HeroesDataParser.Core;

public interface IXmlDataParserProcessor
{
    IEnumerable<ExtractDataOptions> GetAssociatedExtractDataParsers();

    void ExecuteDataParser(ExtractDataOptions option, Map? map);

    Task ExecuteJsonDataFileWriteTasks();
}
