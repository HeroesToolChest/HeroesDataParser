using Heroes.Models;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;

namespace HeroesData.ExtractorData
{
    public class DataHeroSkin : DataExtractorBase<HeroSkin?, HeroSkinParser>, IData
    {
        public DataHeroSkin(HeroSkinParser parser)
            : base(parser)
        {
        }

        public override string Name => "heroskins";

        protected override void Validation(HeroSkin? heroSkin)
        {
            if (heroSkin is null)
                return;

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

            if (string.IsNullOrEmpty(heroSkin.InfoText?.RawDescription))
                AddWarning($"{nameof(heroSkin.InfoText)} is empty");
            else if (heroSkin.InfoText.RawDescription == GameStringParser.FailedParsed)
                AddWarning($"{nameof(heroSkin.InfoText)} failed to parse correctly");
            else if (heroSkin.InfoText.HasErrorTag)
                AddWarning($"{nameof(heroSkin.InfoText)} contains an error tag");

            if (!heroSkin.ReleaseDate.HasValue)
                AddWarning($"{nameof(heroSkin.ReleaseDate)} is null");

            if (heroSkin.Rarity == Rarity.None || heroSkin.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(heroSkin.Rarity)} is {heroSkin.Rarity}");

            if (heroSkin.Franchise == Franchise.Unknown)
                AddWarning($"{nameof(heroSkin.Franchise)} is {heroSkin.Franchise}");
        }
    }
}
