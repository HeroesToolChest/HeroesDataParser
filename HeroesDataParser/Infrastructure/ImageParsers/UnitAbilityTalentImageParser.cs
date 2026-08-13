namespace HeroesDataParser.Infrastructure.ImageParsers;

public class UnitAbilityTalentImageParser : ImageParserBase<Unit>
{
    public UnitAbilityTalentImageParser(ILogger<UnitAbilityTalentImageParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.AbilityTalent;

    protected override string Subdirectory => "abilitytalents";

    protected override void SetImages(Unit element)
    {
        SetAbilityImages(element);
    }

    protected void SetAbilityImages(Unit element)
    {
        foreach (ICollection<Ability> abilityList in element.Abilities.Values)
        {
            SetAbilities(element, abilityList);
        }

        foreach (IDictionary<AbilityTier, IList<Ability>> abilityListByLinkId in element.SubAbilities.Values)
        {
            foreach (ICollection<Ability> abilityList in abilityListByLinkId.Values)
            {
                SetAbilities(element, abilityList);
            }
        }
    }

    private void SetAbilities(Unit unit, ICollection<Ability> abilityList)
    {
        foreach (Ability ability in abilityList)
        {
            string? abilityIcon = ability.Icon;
            ImagePath? abilityIconPath = ability.IconPath;

            if (string.IsNullOrWhiteSpace(abilityIcon) || string.IsNullOrWhiteSpace(abilityIconPath?.FilePath))
                return;

            AddToFiles(abilityIcon, unit.Id, async (directoryPath) =>
            {
                await ProcessStaticImage(abilityIcon, abilityIconPath, directoryPath);
            });
        }
    }
}
