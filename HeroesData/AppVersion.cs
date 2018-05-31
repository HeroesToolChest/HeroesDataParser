using System;
using System.Reflection;

namespace HeroesData
{
    internal static class AppVersion
    {
        public static string GetVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}
