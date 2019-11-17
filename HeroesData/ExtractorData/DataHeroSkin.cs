using Heroes.Models;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;
using System;

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
            {
                throw new ArgumentNullException(nameof(heroSkin));
            }

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
            else if (heroSkin.Description.RawDescription == GameStringParser.FailedParsed)
                AddWarning($"{nameof(heroSkin.Description)} failed to parse correctly");
            else if (heroSkin.Description.HasErrorTag)
                AddWarning($"{nameof(heroSkin.Description)} contains an error tag");

            if (!heroSkin.ReleaseDate.HasValue)
                AddWarning($"{nameof(heroSkin.ReleaseDate)} is null");

            if (heroSkin.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(heroSkin.Rarity)} is unknown");
        }
    }
}
