using Heroes.Models;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;
using System;

namespace HeroesData.ExtractorData
{
    public class DataBanner : DataExtractorBase<Banner?, BannerParser>, IData
    {
        public DataBanner(BannerParser parser)
            : base(parser)
        {
        }

        public override string Name => "banners";

        protected override void Validation(Banner? banner)
        {
            if (banner is null)
            {
                throw new ArgumentNullException(nameof(banner));
            }

            if (string.IsNullOrEmpty(banner.Name))
                AddWarning($"{nameof(banner.Name)} is empty");

            if (string.IsNullOrEmpty(banner.Id))
                AddWarning($"{nameof(banner.Id)} is empty");

            if (string.IsNullOrEmpty(banner.HyperlinkId))
                AddWarning($"{nameof(banner.HyperlinkId)} is empty");

            if (string.IsNullOrEmpty(banner.AttributeId))
                AddWarning($"{nameof(banner.AttributeId)} is empty");

            if (string.IsNullOrEmpty(banner.CollectionCategory))
                AddWarning($"{nameof(banner.CollectionCategory)} is empty");

            if (string.IsNullOrEmpty(banner.Description?.RawDescription))
                AddWarning($"{nameof(banner.Description)} is empty");
            else if (banner.Description.RawDescription == GameStringParser.FailedParsed)
                AddWarning($"{nameof(banner.Description)} failed to parse correctly");
            else if (banner.Description.HasErrorTag)
                AddWarning($"{nameof(banner.Description)} contains an error tag");

            if (!banner.ReleaseDate.HasValue)
                AddWarning($"{nameof(banner.ReleaseDate)} is null");

            if (banner.Rarity == Rarity.None || banner.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(banner.Rarity)} is {banner.Rarity}");
        }
    }
}
