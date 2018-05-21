using HeroesData.FileWriter.Settings;
using System.Xml.Linq;

namespace HeroesData.FileWriter
{
    internal class FileConfiguration
    {
        private readonly string WriterConfigFile = "WriterConfig.xml";

        private XDocument Configuration;

        private FileConfiguration()
        {
            Configuration = XDocument.Load(WriterConfigFile);

            JsonFileSettings = new JsonFileSettings();
            LoadConfig("JsonWriter", JsonFileSettings);

            XmlFileSettings = new XmlFileSettings();
            LoadConfig("XmlWriter", XmlFileSettings);
        }

        public JsonFileSettings JsonFileSettings { get; }
        public XmlFileSettings XmlFileSettings { get; }

        public static FileConfiguration Load()
        {
            return new FileConfiguration();
        }

        private void LoadConfig(string elementName, FileSettings fileSettings)
        {
            XElement writerElement = Configuration.Root.Element(elementName);

            string enabled = writerElement.Attribute("enabled")?.Value;
            if (bool.TryParse(enabled, out bool enabledValue))
                fileSettings.WriterEnabled = enabledValue;
            else
                fileSettings.WriterEnabled = false;

            string fileSplit = writerElement.Element("FileSplit")?.Value;
            if (bool.TryParse(fileSplit, out bool fileSplitValue))
                fileSettings.FileSplit = fileSplitValue;
            else
                fileSettings.FileSplit = false;

            string description = writerElement.Element("Description")?.Value;
            if (int.TryParse(description, out int descriptionValue))
                fileSettings.Description = descriptionValue;
            else
                fileSettings.Description = 5;

            string shortTooltip = writerElement.Element("ShortTooltip")?.Value;
            if (int.TryParse(shortTooltip, out int shortTooltipValue))
                fileSettings.ShortTooltip = shortTooltipValue;
            else
                fileSettings.ShortTooltip = 5;

            string fullTooltip = writerElement.Element("FullTooltip")?.Value;
            if (int.TryParse(fullTooltip, out int fullTooltipValue))
                fileSettings.FullTooltip = fullTooltipValue;
            else
                fileSettings.FullTooltip = 5;

            string imageExtension = writerElement.Element("ImageExtension")?.Value;
            if (!string.IsNullOrEmpty(imageExtension))
                fileSettings.ImageExtension = imageExtension;
            else
                fileSettings.ImageExtension = "dds";

            string includeWeapons = writerElement.Element("IncludeWeapons")?.Value;
            if (bool.TryParse(includeWeapons, out bool includeWeaponsValue))
                fileSettings.IncludeWeapons = includeWeaponsValue;
            else
                fileSettings.IncludeWeapons = false;

            string includeAbilities = writerElement.Element("IncludeAbilities")?.Value;
            if (bool.TryParse(includeAbilities, out bool includeAbilitiesValue))
                fileSettings.IncludeAbilities = includeAbilitiesValue;
            else
                fileSettings.IncludeAbilities = false;

            string includeExtraAbilities = writerElement.Element("IncludeExtraAbilities")?.Value;
            if (bool.TryParse(includeExtraAbilities, out bool includeExtraAbilitiesValue))
                fileSettings.IncludeExtraAbilities = includeExtraAbilitiesValue;
            else
                fileSettings.IncludeExtraAbilities = false;

            string includeTalents = writerElement.Element("IncludeTalents")?.Value;
            if (bool.TryParse(includeTalents, out bool includeTalentsValue))
                fileSettings.IncludeTalents = includeTalentsValue;
            else
                fileSettings.IncludeTalents = false;

            string includeHeroUnits = writerElement.Element("IncludeHeroUnits")?.Value;
            if (bool.TryParse(includeHeroUnits, out bool includeHeroUnitsValue))
                fileSettings.IncludeHeroUnits = includeHeroUnitsValue;
            else
                fileSettings.IncludeHeroUnits = false;
        }
    }
}
