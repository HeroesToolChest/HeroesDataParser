namespace HeroesDataParser.Core;

public interface IAbilityParser
{
    Ability? GetAbility(StormElementData layoutButtonData);

    Ability? GetAbility(string abilityId);
}
