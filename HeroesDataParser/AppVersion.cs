using System.Reflection;

namespace HeroesDataParser;

public static class AppVersion
{
    private static readonly string _appVersion = string.Empty;

    static AppVersion()
    {
        Version? assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
        if (assemblyVersion is not null)
            _appVersion = $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
        else
            _appVersion = "Unknown Version";
    }

    public static string GetAppVersion() => _appVersion;
}
