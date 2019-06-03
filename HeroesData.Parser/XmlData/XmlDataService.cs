using HeroesData.Loader.XmlGameData;

namespace HeroesData.Parser.XmlData
{
    public class XmlDataService : IXmlDataService
    {
        public XmlDataService(Configuration configuration, GameData gameData, DefaultData defaultData)
        {
            Configuration = configuration;
            GameData = gameData;
            DefaultData = defaultData;
        }

        public Configuration Configuration { get; }

        public GameData GameData { get; }

        public DefaultData DefaultData { get; }

        /// <summary>
        /// Gets a new instance of <see cref="WeaponData"/>.
        /// </summary>
        public WeaponData WeaponData => new WeaponData(GameData, DefaultData, Configuration);

        /// <summary>
        /// Gets a new instance of <see cref="ArmorData"/>.
        /// </summary>
        public ArmorData ArmorData => new ArmorData(GameData);

        /// <summary>
        /// Gets a new instance of <see cref="AbilityData"/>.
        /// </summary>
        public AbilityData AbilityData => new AbilityData(GameData, DefaultData, Configuration);

        /// <summary>
        /// Gets a new instance of <see cref="TalentData"/>.
        /// </summary>
        public TalentData TalentData => new TalentData(GameData, DefaultData, Configuration);

        /// <summary>
        /// Gets a new instance of <see cref="UnitData"/>.
        /// </summary>
        public UnitData UnitData => new UnitData(GameData, Configuration, WeaponData, ArmorData, AbilityData);

        public XmlDataService GetInstance()
        {
            return new XmlDataService(Configuration, GameData, DefaultData);
        }
    }
}
