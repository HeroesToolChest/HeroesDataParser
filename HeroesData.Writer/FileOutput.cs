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

        private FileOutput(List<Hero> heroes, string configFileName)
        {
            FileConfiguration = FileConfiguration.Load(configFileName);
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

        private FileOutput(List<Hero> heroes, int? hotsBuild, string configFileName)
        {
            FileConfiguration = FileConfiguration.Load(configFileName);
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

        /// <summary>
        /// Sets the heroes data to be used for the output writer and load the writer configuration file.
        /// </summary>
        /// <param name="heroes">Heroes to be written.</param>
        /// <returns></returns>
        public static FileOutput SetHeroData(List<Hero> heroes)
        {
            return new FileOutput(heroes);
        }

        /// <summary>
        /// Sets the heroes data to be used for the output writer and load the writer configuration file.
        /// </summary>
        /// <param name="heroes">Heroes to be written.</param>
        /// <param name="configFileName">The name of the configuration file to load.</param>
        /// <returns></returns>
        public static FileOutput SetHeroData(List<Hero> heroes, string configFileName)
        {
            return new FileOutput(heroes, configFileName);
        }

        /// <summary>
        /// Sets the heroes data to be used for the output writer and load the writer configuration file.
        /// </summary>
        /// <param name="heroes">Heroes to be written.</param>
        /// <param name="hotsBuild">The current Hots build.</param>
        /// <returns></returns>
        public static FileOutput SetHeroData(List<Hero> heroes, int? hotsBuild)
        {
            return new FileOutput(heroes, hotsBuild);
        }

        /// <summary>
        /// Sets the heroes data to be used for the output writer and load the writer configuration file.
        /// </summary>
        /// <param name="heroes">Heroes to be written.</param>
        /// <param name="hotsBuild">The current Hots build.</param>
        /// <param name="configFileName">The name of the configuration file to load.</param>
        /// <returns></returns>
        public static FileOutput SetHeroData(List<Hero> heroes, int? hotsBuild, string configFileName)
        {
            return new FileOutput(heroes, hotsBuild, configFileName);
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
