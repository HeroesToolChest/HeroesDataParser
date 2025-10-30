namespace HeroesDataParser.Infrastructure.ImageParsers;

public class HeroAbilityTalentImageParser : ImageParserBase<Hero>
{
    public HeroAbilityTalentImageParser(ILogger<HeroAbilityTalentImageParser> logger)
        : base(logger)
    {
    }

    public override ExtractImageOptions ExtractImageOption => ExtractImageOptions.AbilityTalent;

    protected override string SubDirectory => "abilitytalents";

    protected override void SetImages(Hero element, HashSet<ImageWriterPath> imagePaths)
    {
        SetAbilityImages(element, imagePaths);
        SetTalentImages(element, imagePaths);
    }

    protected void SetAbilityImages(Hero element, HashSet<ImageWriterPath> imagePaths)
    {
        foreach (ICollection<Ability> abilityList in element.Abilities.Values)
        {
            foreach (Ability ability in abilityList)
            {
                if (string.IsNullOrWhiteSpace(ability.Icon) || string.IsNullOrWhiteSpace(ability.IconPath?.FilePath))
                    return;

                TryAddToFiles(imagePaths, ability.Icon, ability.IconPath, element);
            }
        }
    }

    protected void SetTalentImages(Hero element, HashSet<ImageWriterPath> imagePaths)
    {
        foreach (ICollection<Talent> talentList in element.Talents.Values)
        {
            foreach (Talent talent in talentList)
            {
                if (string.IsNullOrWhiteSpace(talent.Icon) || string.IsNullOrWhiteSpace(talent.IconPath?.FilePath))
                    return;

                TryAddToFiles(imagePaths, talent.Icon, talent.IconPath, element);
            }
        }
    }
}
