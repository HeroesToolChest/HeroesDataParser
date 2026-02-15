using CASCLib;

namespace HeroesDataParser.Extensions;

public static class CASCConfigExtensions
{
    extension(CASCConfig cascConfig)
    {
        /// <summary>
        /// Gets the <see cref="HeroesDataVersion"/>.
        /// </summary>
        /// <returns><see cref="HeroesDataVersion"/> or <see langword="null"/> if parsing fails.</returns>
        public HeroesDataVersion? GetVersionFromCascConfig()
        {
            string version;

            if (cascConfig.BuildUID.Equals(HeroesXmlLoader.ProductPtrName, StringComparison.OrdinalIgnoreCase))
                version = $"{cascConfig.VersionName}_ptr";
            else
                version = cascConfig.VersionName;

            if (!HeroesDataVersion.TryParse(version, out HeroesDataVersion? parsedVersion))
                return null;

            return parsedVersion;
        }
    }
}