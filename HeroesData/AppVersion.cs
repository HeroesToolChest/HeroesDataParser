using System.Diagnostics;
using System.Reflection;

namespace HeroesData
{
    internal static class AppVersion
    {
        public static string GetVersion()
        {
            return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        }
    }
}
