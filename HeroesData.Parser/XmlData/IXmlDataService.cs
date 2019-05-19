using HeroesData.Loader.XmlGameData;

namespace HeroesData.Parser.XmlData
{
    public interface IXmlDataService
    {
        AbilityData AbilityData { get; }
        ArmorData ArmorData { get; }
        Configuration Configuration { get; }
        DefaultData DefaultData { get; }
        GameData GameData { get; }
        TalentData TalentData { get; }
        WeaponData WeaponData { get; }
    }
}