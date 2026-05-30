namespace HeroesDataParser.Core;

public interface IJsonGameStringFileWriterProcessor
{
    Task WriteGameStringFile(Map? map);

    Task WriteMapGameStringFile();
}
