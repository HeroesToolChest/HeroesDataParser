using HeroesData.Loader.XmlGameData;

namespace HeroesData.Parser.XmlData
{
    public class XmlDataType
    {
        public XmlDataType(Configuration configuration, GameData gameData, DefaultData defaultData)
        {
            Configuration = configuration;
            GameData = gameData;
            DefaultData = defaultData;

            WeaponData = new WeaponData(GameData, DefaultData);
            ArmorData = new ArmorData(GameData);
            AbilityData = new AbilityData(GameData, DefaultData, Configuration);
        }

        public Configuration Configuration { get; }

        public GameData GameData { get; }

        public DefaultData DefaultData { get; }

        public WeaponData WeaponData { get; }

        public ArmorData ArmorData { get; }

        public AbilityData AbilityData { get; }

        public TalentData TalentData { get; }
    }
}
