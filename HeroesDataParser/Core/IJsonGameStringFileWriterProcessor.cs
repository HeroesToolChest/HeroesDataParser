namespace HeroesDataParser.Core;

public interface IJsonGameStringFileWriterProcessor
{
    Task WriteGameStringFile(string? mapName);

    Task WriteMapGameStringFile();
}
