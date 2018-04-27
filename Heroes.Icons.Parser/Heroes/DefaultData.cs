namespace Heroes.Icons.Parser.Heroes
{
    public class DefaultData : HeroData
    {
        public DefaultData(HeroDataLoader heroDataLoader, DescriptionLoader descriptionLoader, DescriptionParser descriptionParser, HeroOverrideLoader heroHelperLoader)
            : base(heroDataLoader, descriptionLoader, descriptionParser, heroHelperLoader)
        {
        }
    }
}
