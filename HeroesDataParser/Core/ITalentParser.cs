using Heroes.Element.Models.AbilityTalents;

namespace HeroesDataParser.Core;

public interface ITalentParser
{
    Talent? GetTalent(Hero hero, StormElementData talentTreeData);
}
