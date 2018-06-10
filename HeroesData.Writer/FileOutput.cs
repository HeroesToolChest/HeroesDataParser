using HeroesData.FileWriter.Writer;
using HeroesData.Parser.Models;
using System.Collections.Generic;

namespace HeroesData.FileWriter
{
    public class FileOutput
    {
        private readonly FileConfiguration FileConfiguration;
        private readonly List<Hero> Heroes;
        private readonly int? HotsBuild;

        private FileOutput(List<Hero> heroes)
        {
            FileConfiguration = FileConfiguration.Load();
            IsXmlEnabled = FileConfiguration.XmlFileSettings.WriterEnabled;
            IsJsonEnabled = FileConfiguration.JsonFileSettings.WriterEnabled;

            Heroes = heroes;
        }

        private FileOutput(List<Hero> heroes, int? hotsBuild)
        {
            FileConfiguration = FileConfiguration.Load();
            IsXmlEnabled = FileConfiguration.XmlFileSettings.WriterEnabled;
            IsJsonEnabled = FileConfiguration.JsonFileSettings.WriterEnabled;
            HotsBuild = hotsBuild;

            Heroes = heroes;
        }

        /// <summary>
        /// Gets whether the xml writer is enabled via the file configuration.
        /// </summary>
        public bool IsXmlEnabled { get; }

        /// <summary>
        /// Gets whether the json writer is enabled via the file configuration.
        /// </summary>
        public bool IsJsonEnabled { get; }

        public static FileOutput SetHeroData(List<Hero> heroes)
        {
            return new FileOutput(heroes);
        }

        public static FileOutput SetHeroData(List<Hero> heroes, int? hotsBuild)
        {
            return new FileOutput(heroes, hotsBuild);
        }

        /// <summary>
        /// Creates the xml output.
        /// </summary>
        public void CreateXml()
        {
            XmlWriter.CreateOutput(FileConfiguration.XmlFileSettings, Heroes, HotsBuild);
        }

        /// <summary>
        /// Creates the xml output.
        /// </summary>
        /// <param name="isEnabled">If true, xml will be created.</param>
        public void CreateXml(bool isEnabled)
        {
            FileConfiguration.XmlFileSettings.WriterEnabled = isEnabled;

            XmlWriter.CreateOutput(FileConfiguration.XmlFileSettings, Heroes, HotsBuild);
        }

        /// <summary>
        /// Creates the Json output.
        /// </summary>
        public void CreateJson()
        {
            JsonWriter.CreateOutput(FileConfiguration.JsonFileSettings, Heroes, HotsBuild);
        }

        /// <summary>
        /// Creates the Json output.
        /// </summary>
        /// <param name="isEnabled">If true, json will be created.</param>
        public void CreateJson(bool isEnabled)
        {
            FileConfiguration.JsonFileSettings.WriterEnabled = isEnabled;

            JsonWriter.CreateOutput(FileConfiguration.JsonFileSettings, Heroes, HotsBuild);
        }
    }
}
