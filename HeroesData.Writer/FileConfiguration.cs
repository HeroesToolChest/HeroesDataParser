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

        private FileConfiguration(string configFileName)
        {
            WriterConfigFile = configFileName;

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

        public static FileConfiguration Load(string configFileName)
        {
            return new FileConfiguration(configFileName);
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
                fileSettings.Description = 0;

            string shortTooltip = writerElement.Element("ShortTooltip")?.Value;
            if (int.TryParse(shortTooltip, out int shortTooltipValue))
                fileSettings.ShortTooltip = shortTooltipValue;
            else
                fileSettings.ShortTooltip = 0;

            string fullTooltip = writerElement.Element("FullTooltip")?.Value;
            if (int.TryParse(fullTooltip, out int fullTooltipValue))
                fileSettings.FullTooltip = fullTooltipValue;
            else
                fileSettings.FullTooltip = 0;

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

            string includeSubAbilities = writerElement.Element("IncludeSubAbilities")?.Value;
            if (bool.TryParse(includeSubAbilities, out bool includeSubAbilitiesValue))
                fileSettings.IncludeSubAbilities = includeSubAbilitiesValue;
            else
                fileSettings.IncludeSubAbilities = false;

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

            // portraits
            string heroSelectPortrait = writerElement.Element("Portrait").Element("HeroSelect")?.Value;
            if (bool.TryParse(heroSelectPortrait, out bool includePortrait))
                fileSettings.HeroSelectPortrait = includePortrait;
            else
                fileSettings.HeroSelectPortrait = false;

            string leaderboardPortrait = writerElement.Element("Portrait").Element("Leaderboard")?.Value;
            if (bool.TryParse(leaderboardPortrait, out includePortrait))
                fileSettings.LeaderboardPortrait = includePortrait;
            else
                fileSettings.LeaderboardPortrait = false;

            string loadingPortrait = writerElement.Element("Portrait").Element("LoadingPortrait")?.Value;
            if (bool.TryParse(loadingPortrait, out includePortrait))
                fileSettings.LoadingPortraitPortrait = includePortrait;
            else
                fileSettings.LoadingPortraitPortrait = false;

            string partyPortrait = writerElement.Element("Portrait").Element("PartyPanel")?.Value;
            if (bool.TryParse(partyPortrait, out includePortrait))
                fileSettings.PartyPanelPortrait = includePortrait;
            else
                fileSettings.PartyPanelPortrait = false;

            string targetPortrait = writerElement.Element("Portrait").Element("Target")?.Value;
            if (bool.TryParse(targetPortrait, out includePortrait))
                fileSettings.TargetPortrait = includePortrait;
            else
                fileSettings.TargetPortrait = false;
        }
    }
}
