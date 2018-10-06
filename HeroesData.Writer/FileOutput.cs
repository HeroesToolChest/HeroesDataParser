using Heroes.Models;
using HeroesData.FileWriter.Writer;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HeroesData.FileWriter
{
    public class FileOutput
    {
        private readonly FileConfiguration FileConfiguration;
        private readonly List<Hero> Heroes;
        private readonly List<MatchAward> MatchAwards;
        private readonly int? HotsBuild;

        public FileOutput(List<Hero> heroes)
        {
            FileConfiguration = FileConfiguration.Load();
            IsXmlEnabled = FileConfiguration.XmlFileSettings.IsWriterEnabled;
            IsJsonEnabled = FileConfiguration.JsonFileSettings.IsWriterEnabled;

            Heroes = heroes;
        }

        public FileOutput(List<Hero> heroes, string configFileName)
        {
            FileConfiguration = FileConfiguration.Load(configFileName);
            IsXmlEnabled = FileConfiguration.XmlFileSettings.IsWriterEnabled;
            IsJsonEnabled = FileConfiguration.JsonFileSettings.IsWriterEnabled;

            Heroes = heroes;
        }

        public FileOutput(List<Hero> heroes, int? hotsBuild)
        {
            FileConfiguration = FileConfiguration.Load();
            IsXmlEnabled = FileConfiguration.XmlFileSettings.IsWriterEnabled;
            IsJsonEnabled = FileConfiguration.JsonFileSettings.IsWriterEnabled;
            HotsBuild = hotsBuild;

            Heroes = heroes;
        }

        public FileOutput(List<Hero> heroes, int? hotsBuild, string configFileName)
        {
            FileConfiguration = FileConfiguration.Load(configFileName);
            IsXmlEnabled = FileConfiguration.XmlFileSettings.IsWriterEnabled;
            IsJsonEnabled = FileConfiguration.JsonFileSettings.IsWriterEnabled;
            HotsBuild = hotsBuild;

            Heroes = heroes;
        }

        public FileOutput(List<MatchAward> matchAwards)
        {
            FileConfiguration = FileConfiguration.Load();
            IsXmlEnabled = FileConfiguration.XmlFileSettings.IsWriterEnabled;
            IsJsonEnabled = FileConfiguration.JsonFileSettings.IsWriterEnabled;

            MatchAwards = matchAwards;
        }

        public FileOutput(List<MatchAward> matchAwards, int? hotsBuild)
        {
            FileConfiguration = FileConfiguration.Load();
            IsXmlEnabled = FileConfiguration.XmlFileSettings.IsWriterEnabled;
            IsJsonEnabled = FileConfiguration.JsonFileSettings.IsWriterEnabled;
            HotsBuild = hotsBuild;

            MatchAwards = matchAwards;
        }

        /// <summary>
        /// Gets whether the xml writer is enabled via the file configuration.
        /// </summary>
        public bool IsXmlEnabled { get; }

        /// <summary>
        /// Gets whether the json writer is enabled via the file configuration.
        /// </summary>
        public bool IsJsonEnabled { get; }

        /// <summary>
        /// Gets or sets the file split option.
        /// </summary>
        public bool FileSplit { get; set; }

        /// <summary>
        /// Gets or sets if localized text is removed from the XML and JSON files.
        /// </summary>
        public bool IsLocalizedText { get; set; }

        /// <summary>
        /// Gets or sets the tooltip description type.
        /// </summary>
        public int DescriptionType { get; set; }

        /// <summary>
        /// Gets or sets the localization string.
        /// </summary>
        public string Localization { get; set; }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        public string OutputDirectory { get; set; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "output");

        /// <summary>
        /// Creates the xml output.
        /// </summary>
        public void CreateXml()
        {
            XmlWriter xmlWriter = new XmlWriter
            {
                FileSettings = FileConfiguration.XmlFileSettings,
                Heroes = Heroes,
                HotsBuild = HotsBuild,
                OutputDirectory = OutputDirectory,
                Localization = Localization,
                MatchAwards = MatchAwards,
            };

            xmlWriter.CreateOutput();
        }

        /// <summary>
        /// Creates the xml output.
        /// </summary>
        /// <param name="isEnabled">If true, xml will be created.</param>
        /// <param name="createLocalizedTextFile">If true, create a localized text file for gamestrings.</param>
        public void CreateXml(bool isEnabled, bool createLocalizedTextFile)
        {
            FileConfiguration.XmlFileSettings.IsWriterEnabled = isEnabled;
            FileConfiguration.XmlFileSettings.IsFileSplit = FileSplit;
            FileConfiguration.XmlFileSettings.Description = DescriptionType;
            FileConfiguration.XmlFileSettings.ShortTooltip = DescriptionType;
            FileConfiguration.XmlFileSettings.FullTooltip = DescriptionType;

            XmlWriter xmlWriter = new XmlWriter
            {
                FileSettings = FileConfiguration.XmlFileSettings,
                Heroes = Heroes,
                HotsBuild = HotsBuild,
                OutputDirectory = OutputDirectory,
                Localization = Localization,
                IsLocalizedText = IsLocalizedText,
                CreateLocalizedTextFile = createLocalizedTextFile,
                MatchAwards = MatchAwards,
            };

            xmlWriter.CreateOutput();
        }

        /// <summary>
        /// Creates the Json output.
        /// </summary>
        public void CreateJson()
        {
            JsonWriter jsonWriter = new JsonWriter
            {
                FileSettings = FileConfiguration.JsonFileSettings,
                Heroes = Heroes,
                HotsBuild = HotsBuild,
                OutputDirectory = OutputDirectory,
                Localization = Localization,
                MatchAwards = MatchAwards,
            };

            jsonWriter.CreateOutput();
        }

        /// <summary>
        /// Creates the Json output.
        /// </summary>
        /// <param name="isEnabled">If true, json will be created.</param>
        /// /// <param name="createLocalizedTextFile">If true, create a localized text file for gamestrings.</param>
        public void CreateJson(bool isEnabled, bool createLocalizedTextFile)
        {
            FileConfiguration.JsonFileSettings.IsWriterEnabled = isEnabled;
            FileConfiguration.JsonFileSettings.IsFileSplit = FileSplit;
            FileConfiguration.JsonFileSettings.Description = DescriptionType;
            FileConfiguration.JsonFileSettings.ShortTooltip = DescriptionType;
            FileConfiguration.JsonFileSettings.FullTooltip = DescriptionType;

            JsonWriter jsonWriter = new JsonWriter
            {
                FileSettings = FileConfiguration.JsonFileSettings,
                Heroes = Heroes,
                HotsBuild = HotsBuild,
                OutputDirectory = OutputDirectory,
                Localization = Localization,
                IsLocalizedText = IsLocalizedText,
                CreateLocalizedTextFile = createLocalizedTextFile,
                MatchAwards = MatchAwards,
            };

            jsonWriter.CreateOutput();
        }
    }
}
