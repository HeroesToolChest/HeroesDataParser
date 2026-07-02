namespace HeroesDataParser.Core;

public interface IAbilityParser
{
    Ability? GetAbility(StormElementData layoutButtonData);

    Ability? GetBehaviorAbility(StormElementData buttonData);

    Ability? GetAbility(string abilityId);

    Ability? GetUnitButtonAbility(string buttonElementId);
}
