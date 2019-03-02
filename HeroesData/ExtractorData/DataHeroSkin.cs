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
                AddWarning($"{nameof(heroSkin.Name)} is null or empty");

            if (string.IsNullOrEmpty(heroSkin.Id))
                AddWarning($"{nameof(heroSkin.Id)} is null or empty");

            if (string.IsNullOrEmpty(heroSkin.HyperlinkId))
                AddWarning($"{nameof(heroSkin.HyperlinkId)} is null or empty");

            if (string.IsNullOrEmpty(heroSkin.SortName))
                AddWarning($"{nameof(heroSkin.SortName)} is null or empty");

            if (string.IsNullOrEmpty(heroSkin.AttributeId))
                AddWarning($"{nameof(heroSkin.AttributeId)} is null or empty");

            if (string.IsNullOrEmpty(heroSkin.Description?.RawDescription))
                AddWarning($"{nameof(heroSkin.Description)} is null or empty");

            if (string.IsNullOrEmpty(heroSkin.SearchText))
                AddWarning($"{nameof(heroSkin.SearchText)} is null or empty");

            if (!heroSkin.ReleaseDate.HasValue)
                AddWarning($"{nameof(heroSkin.ReleaseDate)} is null");
        }
    }
}
