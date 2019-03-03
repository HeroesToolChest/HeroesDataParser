using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataBanner : DataExtractorBase<Banner, BannerParser>, IData
    {
        public DataBanner(BannerParser parser)
            : base(parser)
        {
        }

        public override string Name => "banners";

        protected override void Validation(Banner banner)
        {
            if (string.IsNullOrEmpty(banner.Name))
                AddWarning($"{nameof(banner.Name)} is null or empty");

            if (string.IsNullOrEmpty(banner.Id))
                AddWarning($"{nameof(banner.Id)} is null or empty");

            if (string.IsNullOrEmpty(banner.HyperlinkId))
                AddWarning($"{nameof(banner.HyperlinkId)} is null or empty");

            if (string.IsNullOrEmpty(banner.HyperlinkId))
                AddWarning($"{nameof(banner.HyperlinkId)} is null or empty");

            if (string.IsNullOrEmpty(banner.AttributeId))
                AddWarning($"{nameof(banner.AttributeId)} is null or empty");

            if (string.IsNullOrEmpty(banner.Description?.RawDescription))
                AddWarning($"{nameof(banner.Description)} is null or empty");

            if (!banner.ReleaseDate.HasValue)
                AddWarning($"{nameof(banner.ReleaseDate)} is null");
        }
    }
}
