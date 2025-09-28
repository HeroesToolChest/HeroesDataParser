namespace HeroesDataParser.Infrastructure.ImageParsers;

internal static class AbilityTalentImages
{
    internal static void SetAbilityImages(Hero element, Action<string, ImageRelativePath> addImage)
    {
        foreach (ICollection<Ability> abilityList in element.Abilities.Values)
        {
            foreach (Ability ability in abilityList)
            {
                if (string.IsNullOrWhiteSpace(ability.Icon) || string.IsNullOrWhiteSpace(ability.IconPath?.FilePath))
                    return;

                addImage.Invoke(ability.Icon, new ImageRelativePath(element, ability.IconPath));
            }
        }
    }

    internal static void SetTalentImages(Hero element, Action<string, ImageRelativePath> addImage)
    {
        foreach (ICollection<Talent> talentList in element.Talents.Values)
        {
            foreach (Talent talent in talentList)
            {
                if (string.IsNullOrWhiteSpace(talent.Icon) || string.IsNullOrWhiteSpace(talent.IconPath?.FilePath))
                    return;

                addImage.Invoke(talent.Icon, new ImageRelativePath(element, talent.IconPath));
            }
        }
    }
}
