namespace HeroesDataParser;

// paths for the creation of files and directories; logs, casc cdn cache, etc.
public static class AppPaths
{
    public static string GetDirectory(string subDirectory)
    {
        string baseDirectory = ResolveBaseDirectory();
        string fullPath = Path.Combine(baseDirectory, subDirectory);

        Directory.CreateDirectory(fullPath);

        return fullPath;
    }

    private static string ResolveBaseDirectory()
    {
        string? environmentalPath = Environment.GetEnvironmentVariable($"{Constants.EnvironmentVariablePrefix}DATA_DIR");
        if (!string.IsNullOrWhiteSpace(environmentalPath))
            return environmentalPath;

        string exeDirectory = AppContext.BaseDirectory;

#if DEBUG
        return exeDirectory;
#else
        // for development/zip installs
        if (File.Exists(Path.Combine(exeDirectory, Constants.PortableMarkerFileName)))
            return exeDirectory;

        // tool install
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create);
        return Path.Combine(localAppData, Constants.AppNameShort);
#endif
    }
}
