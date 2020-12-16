using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataSpray : DataExtractorBase<Spray?, SprayParser>, IData
    {
        public DataSpray(SprayParser parser)
            : base(parser)
        {
        }

        public override string Name => "sprays";

        protected override void Validation(Spray? data)
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

            if (!data.ReleaseDate.HasValue)
                AddWarning($"{nameof(data.ReleaseDate)} is null");

            if (data.Rarity == Rarity.None || data.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(data.Rarity)} is {data.Rarity}");

            if (string.IsNullOrEmpty(data.TextureSheet.Image))
                AddWarning($"{nameof(data.TextureSheet.Image)} is empty");
        }
    }
}
