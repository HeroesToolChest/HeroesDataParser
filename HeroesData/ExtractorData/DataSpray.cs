using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataSpray : DataExtractorBase<Spray, SprayParser>, IData
    {
        public DataSpray(SprayParser parser)
            : base(parser)
        {
        }

        public override string Name => "sprays";

        protected override void Validation(Spray spray)
        {
            if (string.IsNullOrEmpty(spray.Name))
                AddWarning($"{nameof(spray.Name)} is null or empty");

            if (string.IsNullOrEmpty(spray.Id))
                AddWarning($"{nameof(spray.Id)} is null or empty");

            if (string.IsNullOrEmpty(spray.HyperlinkId))
                AddWarning($"{nameof(spray.HyperlinkId)} is null or empty");

            if (string.IsNullOrEmpty(spray.HyperlinkId))
                AddWarning($"{nameof(spray.HyperlinkId)} is null or empty");

            if (string.IsNullOrEmpty(spray.AttributeId))
                AddWarning($"{nameof(spray.AttributeId)} is null or empty");

            if (string.IsNullOrEmpty(spray.SearchText))
                AddWarning($"{nameof(spray.SearchText)} is null or empty");

            if (!spray.ReleaseDate.HasValue)
                AddWarning($"{nameof(spray.ReleaseDate)} is null");
        }
    }
}
