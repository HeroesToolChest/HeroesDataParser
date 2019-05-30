using HeroesData.Loader.XmlGameData;

namespace HeroesData.Parser.XmlData
{
    public interface IXmlDataService
    {
        AbilityData AbilityData { get; }
        ArmorData ArmorData { get; }
        BehaviorData BehaviorData { get; }
        Configuration Configuration { get; }
        DefaultData DefaultData { get; }
        GameData GameData { get; }
        TalentData TalentData { get; }
        WeaponData WeaponData { get; }
        UnitData UnitData { get; }

        XmlDataService GetInstance();
    }
}