using Heroes.Models;
using HeroesData.FileWriter.Writers;
using HeroesData.FileWriter.Writers.AnnouncerData;
using HeroesData.FileWriter.Writers.BannerData;
using HeroesData.FileWriter.Writers.BehaviorVeterancyData;
using HeroesData.FileWriter.Writers.BoostData;
using HeroesData.FileWriter.Writers.BundleData;
using HeroesData.FileWriter.Writers.EmoticonData;
using HeroesData.FileWriter.Writers.EmoticonPackData;
using HeroesData.FileWriter.Writers.HeroData;
using HeroesData.FileWriter.Writers.HeroSkinData;
using HeroesData.FileWriter.Writers.LootChestData;
using HeroesData.FileWriter.Writers.MatchAwardData;
using HeroesData.FileWriter.Writers.MountData;
using HeroesData.FileWriter.Writers.PortraitPackData;
using HeroesData.FileWriter.Writers.RewardPortraitData;
using HeroesData.FileWriter.Writers.SprayData;
using HeroesData.FileWriter.Writers.TypeDescriptionData;
using HeroesData.FileWriter.Writers.UnitData;
using HeroesData.FileWriter.Writers.VoiceLineData;
using System.Collections.Generic;

namespace HeroesData.FileWriter
{
    public class FileOutput
    {
        private readonly FileOutputOptions _fileOutputOptions;
        private readonly int? _hotsBuild;

        private readonly Dictionary<FileOutputType, Dictionary<string, IWritable>> _writers = new Dictionary<FileOutputType, Dictionary<string, IWritable>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileOutput"/> class.
        /// Creates the output files.
        /// </summary>
        public FileOutput()
        {
            _fileOutputOptions = new FileOutputOptions();

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileOutput"/> class.
        /// Creates the output files.
        /// </summary>
        /// <param name="hotsBuild">The hots build number.</param>
        public FileOutput(int? hotsBuild)
        {
            _hotsBuild = hotsBuild;
            _fileOutputOptions = new FileOutputOptions();

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileOutput"/> class.
        /// Creates the output files.
        /// </summary>
        /// <param name="fileOutputOptions">Configuration options that can be set from the CLI.</param>
        public FileOutput(FileOutputOptions fileOutputOptions)
        {
            _fileOutputOptions = fileOutputOptions;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileOutput"/> class.
        /// Creates the output files.
        /// </summary>
        /// <param name="hotsBuild">The hots build number.</param>
        /// <param name="fileOutputOptions">Configuration options that can be set from the CLI.</param>
        public FileOutput(int? hotsBuild, FileOutputOptions fileOutputOptions)
        {
            _hotsBuild = hotsBuild;
            _fileOutputOptions = fileOutputOptions;

            Initialize();
        }

        /// <summary>
        /// Creates the output files.
        /// </summary>
        /// <typeparam name="T">A type of <see cref="IExtractable"/>.</typeparam>
        /// <param name="items">The parsed items to be used for the creation of the output.</param>
        /// <param name="fileOutputType">The file type.</param>
        /// <returns></returns>
        public bool Create<T>(IEnumerable<T> items, FileOutputType fileOutputType)
            where T : IExtractable
        {
            if (_writers[fileOutputType].TryGetValue(typeof(T).Name, out IWritable? writable))
            {
                writable.FileOutputOptions = _fileOutputOptions;
                writable.HotsBuild = _hotsBuild;

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
            _writers.Add(FileOutputType.Json, new Dictionary<string, IWritable>()
            {
                { nameof(Hero), new HeroDataJsonWriter() },
                { nameof(Unit), new UnitDataJsonWriter() },
                { nameof(MatchAward), new MatchAwardDataJsonWriter() },
                { nameof(HeroSkin), new HeroSkinDataJsonWriter() },
                { nameof(Mount), new MountDataJsonWriter() },
                { nameof(Banner), new BannerDataJsonWriter() },
                { nameof(Spray), new SprayDataJsonWriter() },
                { nameof(Announcer), new AnnouncerDataJsonWriter() },
                { nameof(VoiceLine), new VoiceLineDataJsonWriter() },
                { nameof(PortraitPack), new PortraitPackDataJsonWriter() },
                { nameof(RewardPortrait), new RewardPortraitDataJsonWriter() },
                { nameof(Emoticon), new EmoticonDataJsonWriter() },
                { nameof(EmoticonPack), new EmoticonPackDataJsonWriter() },
                { nameof(BehaviorVeterancy), new BehaviorVeterancyDataJsonWriter() },
                { nameof(Bundle), new BundleDataJsonWriter() },
                { nameof(Boost), new BoostDataJsonWriter() },
                { nameof(LootChest), new LootChestDataJsonWriter() },
                { nameof(TypeDescription), new TypeDescriptionDataJsonWriter() },
            });

            _writers.Add(FileOutputType.Xml, new Dictionary<string, IWritable>()
            {
                { nameof(Hero), new HeroDataXmlWriter() },
                { nameof(Unit), new UnitDataXmlWriter() },
                { nameof(MatchAward), new MatchAwardDataXmlWriter() },
                { nameof(HeroSkin), new HeroSkinDataXmlWriter() },
                { nameof(Mount), new MountDataXmlWriter() },
                { nameof(Banner), new BannerDataXmlWriter() },
                { nameof(Spray), new SprayDataXmlWriter() },
                { nameof(Announcer), new AnnouncerDataXmlWriter() },
                { nameof(VoiceLine), new VoiceLineDataXmlWriter() },
                { nameof(PortraitPack), new PortraitPackDataXmlWriter() },
                { nameof(RewardPortrait), new RewardPortraitDataXmlWriter() },
                { nameof(Emoticon), new EmoticonDataXmlWriter() },
                { nameof(EmoticonPack), new EmoticonPackDataXmlWriter() },
                { nameof(BehaviorVeterancy), new BehaviorVeterancyDataXmlWriter() },
                { nameof(Bundle), new BundleDataXmlWriter() },
                { nameof(Boost), new BoostDataXmlWriter() },
                { nameof(LootChest), new LootChestDataXmlWriter() },
                { nameof(TypeDescription), new TypeDescriptionDataXmlWriter() },
            });
        }
    }
}
