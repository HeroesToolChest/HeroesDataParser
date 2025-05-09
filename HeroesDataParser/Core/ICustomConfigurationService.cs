namespace HeroesDataParser.Core;

public interface ICustomConfigurationService
{
    ISet<string> SelectedCustomDataFilePaths { get; }

    string CustomConfigurationDirectory { get; }

    void Load();
}
