namespace HeroesDataParser.Infrastructure.ImageParsers;

public class HeroAbilityTalentImageParser : ImageParserBase<Hero>
{
    public HeroAbilityTalentImageParser(ILogger<HeroAbilityTalentImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.AbilityTalent;

    protected override string SubDirectory => "abilitytalents";

    protected override void SetImages(Hero element)
    {
        SetAbilityImages(element);
        SetTalentImages(element);
    }

    protected void SetAbilityImages(Hero element)
    {
        foreach (ICollection<Ability> abilityList in element.Abilities.Values)
        {
            foreach (Ability ability in abilityList)
            {
                string? abilityIcon = ability.Icon;
                RelativeFilePath? abilityIconPath = ability.IconPath;

                if (string.IsNullOrWhiteSpace(abilityIcon) || string.IsNullOrWhiteSpace(abilityIconPath?.FilePath))
                    return;

                TryAddToFiles(abilityIcon, element.Id, async (directoryPath) =>
                {
                    await ProcessBasicImage(abilityIcon, abilityIconPath, directoryPath);
                });
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
                RelativeFilePath? talentIconPath = talent.IconPath;

                if (string.IsNullOrWhiteSpace(talentIcon) || string.IsNullOrWhiteSpace(talentIconPath?.FilePath))
                    return;

                TryAddToFiles(talentIcon, element.Id, async (directoryPath) =>
                {
                    await ProcessBasicImage(talentIcon, talentIconPath, directoryPath);
                });
            }
        }
    }
}
