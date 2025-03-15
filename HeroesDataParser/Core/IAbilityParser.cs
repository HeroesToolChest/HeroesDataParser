using Heroes.Element.Models.AbilityTalents;

namespace HeroesDataParser.Core;

public interface IAbilityParser
{
    Ability? GetAbility(StormElementData layoutButtonData);

    Ability? GetAbility(string abilityId);
}
