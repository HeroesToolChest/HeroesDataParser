using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataHeroSkin : DataExtractorBase<HeroSkin, HeroSkinParser>, IData
    {
        public DataHeroSkin(HeroSkinParser parser)
            : base(parser)
        {
        }

        public override string Name => "heroskins";

        protected override void Validation(HeroSkin heroSkin)
        {
            if (string.IsNullOrEmpty(heroSkin.Name))
                AddWarning($"{nameof(heroSkin.Name)} is empty");

            if (string.IsNullOrEmpty(heroSkin.Id))
                AddWarning($"{nameof(heroSkin.Id)} is empty");

            if (string.IsNullOrEmpty(heroSkin.HyperlinkId))
                AddWarning($"{nameof(heroSkin.HyperlinkId)} is empty");

            if (string.IsNullOrEmpty(heroSkin.SortName))
                AddWarning($"{nameof(heroSkin.SortName)} is empty");

            if (string.IsNullOrEmpty(heroSkin.AttributeId))
                AddWarning($"{nameof(heroSkin.AttributeId)} is empty");

            if (string.IsNullOrEmpty(heroSkin.Description?.RawDescription))
                AddWarning($"{nameof(heroSkin.Description)} is empty");

            if (!heroSkin.ReleaseDate.HasValue)
                AddWarning($"{nameof(heroSkin.ReleaseDate)} is null");

            if (heroSkin.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(heroSkin.Rarity)} is unknown");
        }
    }
}
