namespace HeroesDataParser.Infrastructure.ImageParsers;

public class UnitAbilityTalentImageParser : ImageParserBase<Unit>
{
    public UnitAbilityTalentImageParser(ILogger<UnitAbilityTalentImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.AbilityTalent;

    protected override string SubDirectory => "abilitytalents";

    protected override void SetImages(Unit element)
    {
        SetAbilityImages(element);
    }

    protected void SetAbilityImages(Unit element)
    {
        foreach (ICollection<Ability> abilityList in element.Abilities.Values)
        {
            foreach (Ability ability in abilityList)
            {
                string? abilityIcon = ability.Icon;
                RelativeFilePath? abilityIconPath = ability.IconPath;

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
