namespace HeroesDataParser.Cli.Commands.PortraitCommands;

public static class PortraitCommandHelpers
{
    public static void SetBattleNetCacheDirectory(IAnsiConsole console, PortraitCacheOptions options)
    {
        string programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        if (string.IsNullOrWhiteSpace(programDataPath))
        {
            console.WriteLine("Could not find the common application data folder");
            return;
        }

        string cachePath = Path.Combine(programDataPath, "Blizzard Entertainment", "Battle.net", "Cache");

        if (!Directory.Exists(cachePath))
        {
            console.WriteLine("Could not find the Battle.net cache directory");
            return;
        }

        options.BattleNetCacheDirectory = cachePath;
    }
}
