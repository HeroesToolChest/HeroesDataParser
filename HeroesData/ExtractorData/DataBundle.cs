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

        protected override void Validation(Bundle? bundle)
        {
            if (bundle is null)
                return;

            if (string.IsNullOrEmpty(bundle.Name))
                AddWarning($"{nameof(bundle.Name)} is empty");

            if (string.IsNullOrEmpty(bundle.Id))
                AddWarning($"{nameof(bundle.Id)} is empty");

            if (string.IsNullOrEmpty(bundle.HyperlinkId))
                AddWarning($"{nameof(bundle.HyperlinkId)} is empty");

            if (string.IsNullOrEmpty(bundle.SortName))
                AddWarning($"{nameof(bundle.SortName)} is empty");

            if (!bundle.ReleaseDate.HasValue)
                AddWarning($"{nameof(bundle.ReleaseDate)} is null");

            if (string.IsNullOrEmpty(bundle.ImageFileName))
                AddWarning($"{nameof(bundle.ImageFileName)} is empty");
        }
    }
}
