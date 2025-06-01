namespace HeroesDataParser.Core;

public interface ITalentParser
{
    Talent? GetTalent(Hero hero, StormElementData talentTreeData);
}
