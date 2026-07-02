namespace HeroesDataParser.Core;

public interface ICustomConfigurationService
{
    IReadOnlyList<string> SelectedCustomDataFilePaths { get; }

    string CustomConfigurationDirectory { get; }

    void Load();
}
