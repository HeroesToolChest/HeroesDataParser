using Heroes.Icons.Parser.Descriptions;
using Heroes.Icons.Parser.HeroData;

namespace Heroes.Icons.Parser.Heroes
{
    public class DefaultData : DefaultHeroData
    {
        public DefaultData(HeroDataLoader heroDataLoader, DescriptionLoader descriptionLoader, DescriptionParser descriptionParser, HeroOverrideLoader heroHelperLoader)
            : base(heroDataLoader, descriptionLoader, descriptionParser, heroHelperLoader)
        {
        }
    }
}
