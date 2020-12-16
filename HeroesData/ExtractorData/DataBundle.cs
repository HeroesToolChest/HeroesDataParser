using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataBundle : DataExtractorBase<Bundle?, BundleParser>, IData
    {
        public DataBundle(BundleParser parser)
            : base(parser)
        {
        }

        public override string Name => "bundles";

        protected override void Validation(Bundle? data)
        {
            if (data is null)
                return;

            if (string.IsNullOrEmpty(data.Id))
                AddWarning($"{nameof(data.Id)} is empty");

            if (string.IsNullOrEmpty(data.HyperlinkId))
                AddWarning($"{nameof(data.HyperlinkId)} is empty");

            if (!data.ReleaseDate.HasValue)
                AddWarning($"{nameof(data.ReleaseDate)} is null");
        }
    }
}
