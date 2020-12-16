using Heroes.Models;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;

namespace HeroesData.ExtractorData
{
    public class DataBanner : DataExtractorBase<Banner?, BannerParser>, IData
    {
        public DataBanner(BannerParser parser)
            : base(parser)
        {
        }

        public override string Name => "banners";

        protected override void Validation(Banner? data)
        {
            if (data is null)
                return;

            if (string.IsNullOrEmpty(data.Name))
                AddWarning($"{nameof(data.Name)} is empty");

            if (string.IsNullOrEmpty(data.Id))
                AddWarning($"{nameof(data.Id)} is empty");

            if (string.IsNullOrEmpty(data.HyperlinkId))
                AddWarning($"{nameof(data.HyperlinkId)} is empty");

            if (string.IsNullOrEmpty(data.AttributeId))
                AddWarning($"{nameof(data.AttributeId)} is empty");

            if (string.IsNullOrEmpty(data.CollectionCategory))
                AddWarning($"{nameof(data.CollectionCategory)} is empty");

            if (string.IsNullOrEmpty(data.Description?.RawDescription))
                AddWarning($"{nameof(data.Description)} is empty");
            else if (data.Description.RawDescription == GameStringParser.FailedParsed)
                AddWarning($"{nameof(data.Description)} failed to parse correctly");
            else if (data.Description.HasErrorTag)
                AddWarning($"{nameof(data.Description)} contains an error tag");

            if (!data.ReleaseDate.HasValue)
                AddWarning($"{nameof(data.ReleaseDate)} is null");

            if (data.Rarity == Rarity.None || data.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(data.Rarity)} is {data.Rarity}");
        }
    }
}
