namespace HeroesDataParser.Cli.Commands.PortraitCommands;

public static class PortraitCommandHelpers
{
    public static bool SetBattleNetCacheDirectory(IAnsiConsole console, PortraitCacheOptions options)
    {
        string programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        if (string.IsNullOrWhiteSpace(programDataPath))
        {
            console.MarkupLine("[red]Could not find the common application data folder[/]");
            return false;
        }

        string cachePath = Path.Combine(programDataPath, "Blizzard Entertainment", "Battle.net", "Cache");

        if (!Directory.Exists(cachePath))
        {
            console.MarkupLine("[red]Could not find the Battle.net cache directory[/]");
            return false;
        }

        options.BattleNetCacheDirectory = cachePath;

        return true;
    }
}
