using Heroes.Icons.Parser.Descriptions;
using Heroes.Icons.Parser.HeroData;

namespace Heroes.Icons.Parser.Heroes
{
    public class DataLoader
    {
        public DataLoader(HeroDataLoader heroDataLoader, DescriptionLoader descriptionLoader, HeroOverrideLoader heroOverrideLoader, ScalingDataLoader scalingDataLoader)
        {
            HeroDataLoader = heroDataLoader;
            DescriptionLoader = descriptionLoader;
            HeroOverrideLoader = heroOverrideLoader;
            ScalingDataLoader = scalingDataLoader;
        }

        public HeroDataLoader HeroDataLoader { get; }
        public DescriptionLoader DescriptionLoader { get; }
        public HeroOverrideLoader HeroOverrideLoader { get; }
        public ScalingDataLoader ScalingDataLoader { get; }
    }
}
