namespace HeroesDataParser.Core;

public interface IParsingConfigurationService
{
    string? SelectedFilePath { get; }

    string ParsingConfigurationDirectory { get; }

    void Load();

    IEnumerable<string> FilterAllowedItems(string dataObjectType, IEnumerable<string> items);
}
