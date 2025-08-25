using System.IO.Abstractions;

namespace HeroesDataParser.Infrastructure.ImageWriters;

internal static class AbilityTalentImages
{
    internal static void SetAbilityImages(Hero element, Dictionary<string, ImageRelativePath> heroAbilityRelativePathsByFileName)
    {
        foreach (IReadOnlyList<Ability> abilityList in element.Abilities.Values)
        {
            foreach (Ability ability in abilityList)
            {
                if (string.IsNullOrWhiteSpace(ability.Icon) || string.IsNullOrWhiteSpace(ability.IconPath?.FilePath))
                    return;

                heroAbilityRelativePathsByFileName.TryAdd(ability.Icon, new ImageRelativePath(element, ability.IconPath));
            }
        }
    }

    internal static void SetTalentImages(Hero element, Dictionary<string, ImageRelativePath> heroTalentRelativePathsByFileName)
    {
        foreach (IReadOnlyList<Talent> talentList in element.Talents.Values)
        {
            foreach (Talent talent in talentList)
            {
                if (string.IsNullOrWhiteSpace(talent.Icon) || string.IsNullOrWhiteSpace(talent.IconPath?.FilePath))
                    return;

                heroTalentRelativePathsByFileName.TryAdd(talent.Icon, new ImageRelativePath(element, talent.IconPath));
            }
        }
    }
}
