using Heroes.Element.Models.AbilityTalents;

namespace HeroesDataParser.Core;

public interface IAbilityParser
{
    Ability? GetAbility(string unitId, StormElementData layoutButtonData);
}
