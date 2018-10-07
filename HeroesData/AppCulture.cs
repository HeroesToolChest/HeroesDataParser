using System.Globalization;

namespace HeroesData
{
    internal static class AppCulture
    {
        public static void SetCurrentCulture()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }
}
