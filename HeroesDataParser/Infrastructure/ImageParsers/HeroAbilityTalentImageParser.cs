namespace HeroesDataParser.Infrastructure.ImageParsers;

public class HeroAbilityTalentImageParser : ImageParserBase<Hero>
{
    public HeroAbilityTalentImageParser(ILogger<HeroAbilityTalentImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.AbilityTalent;

    protected override string Subdirectory => "abilitytalents";

    protected override void SetImages(Hero element)
    {
        SetAbilityImages(element);
        SetHeroUnitAbilityImages(element);
        SetTalentImages(element);
    }

    protected void SetAbilityImages(Hero element)
    {
        foreach (ICollection<Ability> abilityList in element.Abilities.Values)
        {
            foreach (Ability ability in abilityList)
            {
                string? abilityIcon = ability.Icon;
                ImagePath? abilityIconPath = ability.IconPath;

                if (string.IsNullOrWhiteSpace(abilityIcon) || string.IsNullOrWhiteSpace(abilityIconPath?.FilePath))
                    return;

                AddToFiles(abilityIcon, element.Id, async (directoryPath) =>
                {
                    await ProcessStaticImage(abilityIcon, abilityIconPath, directoryPath);
                });
            }
        }
    }

    protected void SetHeroUnitAbilityImages(Hero element)
    {
        foreach (var heroUnit in element.HeroUnits)
        {
            foreach (ICollection<Ability> abilityList in heroUnit.Value.Abilities.Values)
            {
                foreach (Ability ability in abilityList)
                {
                    string? abilityIcon = ability.Icon;
                    ImagePath? abilityIconPath = ability.IconPath;

                    if (string.IsNullOrWhiteSpace(abilityIcon) || string.IsNullOrWhiteSpace(abilityIconPath?.FilePath))
                        return;

                    AddToFiles(abilityIcon, element.Id, async (directoryPath) =>
                    {
                        await ProcessStaticImage(abilityIcon, abilityIconPath, directoryPath);
                    });
                }
            }
        }
    }

    protected void SetTalentImages(Hero element)
    {
        foreach (ICollection<Talent> talentList in element.Talents.Values)
        {
            foreach (Talent talent in talentList)
            {
                string? talentIcon = talent.Icon;
                ImagePath? talentIconPath = talent.IconPath;

                if (string.IsNullOrWhiteSpace(talentIcon) || string.IsNullOrWhiteSpace(talentIconPath?.FilePath))
                    return;

                AddToFiles(talentIcon, element.Id, async (directoryPath) =>
                {
                    await ProcessStaticImage(talentIcon, talentIconPath, directoryPath);
                });
            }
        }
    }
}
