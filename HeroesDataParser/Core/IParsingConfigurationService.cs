namespace HeroesDataParser.Core;

public interface IParsingConfigurationService
{
    string? SelectedFilePath { get; }

    string ParsingConfigurationDirectory { get; }

    IEnumerable<string> FilterAllowedItems(string dataObjectType, IEnumerable<string> items);
}
