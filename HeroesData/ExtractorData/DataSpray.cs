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
                AddWarning($"{nameof(spray.Name)} is empty");

            if (string.IsNullOrEmpty(spray.Id))
                AddWarning($"{nameof(spray.Id)} is empty");

            if (string.IsNullOrEmpty(spray.HyperlinkId))
                AddWarning($"{nameof(spray.HyperlinkId)} is empty");

            if (string.IsNullOrEmpty(spray.AttributeId))
                AddWarning($"{nameof(spray.AttributeId)} is empty");

            if (string.IsNullOrEmpty(spray.CollectionCategory))
                AddWarning($"{nameof(spray.CollectionCategory)} is empty");

            if (!spray.ReleaseDate.HasValue)
                AddWarning($"{nameof(spray.ReleaseDate)} is null");

            if (spray.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(spray.Rarity)} is unknown");

            if (string.IsNullOrEmpty(spray.ImageFileName))
                AddWarning($"{nameof(spray.ImageFileName)} is empty");
        }
    }
}
