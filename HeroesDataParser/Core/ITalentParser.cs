namespace HeroesDataParser.Core;

public interface ITalentParser
{
    Talent? GetTalent(Hero hero, StormElementData talentTreeData);

    List<Ability> GetBehaviorAbilitiesFromTalent(Talent talent);
}
