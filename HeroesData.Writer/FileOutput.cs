using Heroes.Models;
using HeroesData.FileWriter.Settings;
using HeroesData.FileWriter.Writers;
using HeroesData.FileWriter.Writers.HeroData;
using HeroesData.FileWriter.Writers.MatchAwardData;
using System.Collections.Generic;

namespace HeroesData.FileWriter
{
    public class FileOutput
    {
        private readonly FileConfiguration FileConfiguration; // config file
        private readonly FileOutputOptions FileOutputOptions; // cli
        private readonly int? HotsBuild;

        private Dictionary<FileOutputType, Dictionary<string, IWritable>> Writers = new Dictionary<FileOutputType, Dictionary<string, IWritable>>();

        /// <summary>
        /// Creates the output files.
        /// </summary>
        public FileOutput()
        {
            FileConfiguration = FileConfiguration.Load();
            FileOutputOptions = new FileOutputOptions();

            Initialize();
        }

        /// <summary>
        /// Creates the output files.
        /// </summary>
        /// <param name="configFileName">The file name of the xml configuration file.</param>
        public FileOutput(string configFileName)
        {
            FileConfiguration = FileConfiguration.Load(configFileName);
            FileOutputOptions = new FileOutputOptions();

            Initialize();
        }

        /// <summary>
        /// Creates the output files.
        /// </summary>
        /// <param name="hotsBuild">The hots build number.</param>
        public FileOutput(int? hotsBuild)
        {
            FileConfiguration = FileConfiguration.Load();
            HotsBuild = hotsBuild;
            FileOutputOptions = new FileOutputOptions();

            Initialize();
        }

        /// <summary>
        /// Creates the output files.
        /// </summary>
        /// <param name="hotsBuild">The hots build number.</param>
        /// <param name="configFileName">The file name of the xml configuration file.</param>
        public FileOutput(int? hotsBuild, string configFileName)
        {
            FileConfiguration = FileConfiguration.Load(configFileName);
            HotsBuild = hotsBuild;
            FileOutputOptions = new FileOutputOptions();

            Initialize();
        }

        /// <summary>
        /// Creates the output files.
        /// </summary>
        /// <param name="fileOutputOptions">Configuration options that can be set from the CLI.</param>
        public FileOutput(FileOutputOptions fileOutputOptions)
        {
            FileConfiguration = FileConfiguration.Load();
            FileOutputOptions = fileOutputOptions;

            Initialize();
        }

        /// <summary>
        /// Creates the output files.
        /// </summary>
        /// <param name="configFileName">The file name of the xml configuration file.</param>
        /// <param name="fileOutputOptions">Configuration options that can be set from the CLI.</param>
        public FileOutput(string configFileName, FileOutputOptions fileOutputOptions)
        {
            FileConfiguration = FileConfiguration.Load(configFileName);
            FileOutputOptions = fileOutputOptions;

            Initialize();
        }

        /// <summary>
        /// Creates the output files.
        /// </summary>
        /// <param name="hotsBuild">The hots build number.</param>
        /// <param name="fileOutputOptions">Configuration options that can be set from the CLI.</param>
        public FileOutput(int? hotsBuild, FileOutputOptions fileOutputOptions)
        {
            FileConfiguration = FileConfiguration.Load();
            HotsBuild = hotsBuild;
            FileOutputOptions = fileOutputOptions;

            Initialize();
        }

        /// <summary>
        /// Creates the output files.
        /// </summary>
        /// <param name="hotsBuild">The hots build number.</param>
        /// <param name="configFileName">The file name of the xml configuration file.</param>
        /// <param name="fileOutputOptions">Configuration options that can be set from the CLI.</param>
        public FileOutput(int? hotsBuild, string configFileName, FileOutputOptions fileOutputOptions)
        {
            FileConfiguration = FileConfiguration.Load(configFileName);
            HotsBuild = hotsBuild;
            FileOutputOptions = fileOutputOptions;

            Initialize();
        }

        /// <summary>
        /// Creates the output files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The parsed items to be used for the creation of the output.</param>
        /// <param name="fileOutputType">The file type.</param>
        /// <returns></returns>
        public bool Create<T>(IEnumerable<T> items, FileOutputType fileOutputType)
            where T : IExtractable
        {
            if (Writers[fileOutputType].TryGetValue(typeof(T).Name, out IWritable writable))
            {
                writable.BaseDirectory = FileOutputOptions.OutputDirectory;
                writable.IsLocalizedText = FileOutputOptions.IsLocalizedText;
                writable.IsMinifiedFiles = FileOutputOptions.IsMinifiedFiles;
                writable.Localization = FileOutputOptions.Localization;
                writable.HotsBuild = HotsBuild;
                writable.FileSettings = new FileSettings
                {
                    IsFileSplit = FileOutputOptions.IsFileSplit ?? false,
                    DescriptionType = FileOutputOptions.DescriptionType ?? 0,
                };

                ((IWriter<T>)writable).CreateOutput(items);

                return true;
            }

            return false;
        }

        private void Initialize()
        {
            SetWriters();
        }

        private void SetWriters()
        {
            Writers.Add(FileOutputType.Json, new Dictionary<string, IWritable>()
            {
               { nameof(Hero), new HeroDataJsonWriter() },
               { nameof(MatchAward), new MatchAwardDataJsonWriter() },
            });

            Writers.Add(FileOutputType.Xml, new Dictionary<string, IWritable>()
            {
               { nameof(Hero), new HeroDataXmlWriter() },
               { nameof(MatchAward), new MatchAwardDataXmlWriter() },
            });
        }
    }
}
