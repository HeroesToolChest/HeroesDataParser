using Heroes.Models;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;

namespace HeroesData.ExtractorData
{
    public class DataMount : DataExtractorBase<Mount?, MountParser>, IData
    {
        public DataMount(MountParser parser)
            : base(parser)
        {
        }

        public override string Name => "mounts";

        protected override void Validation(Mount? data)
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

            if (string.IsNullOrEmpty(data.MountCategory))
                AddWarning($"{nameof(data.MountCategory)} is empty");

            if (string.IsNullOrEmpty(data.CollectionCategory))
                AddWarning($"{nameof(data.CollectionCategory)} is empty");

            if (data.MountCategory == "Unique")
            {
                if (string.IsNullOrEmpty(data.InfoText?.RawDescription))
                    AddWarning($"{nameof(data.InfoText)} is empty");
                else if (data.InfoText.RawDescription == GameStringParser.FailedParsed)
                    AddWarning($"{nameof(data.InfoText)} failed to parse correctly");
                else if (data.InfoText.HasErrorTag)
                    AddWarning($"{nameof(data.InfoText)} contains an error tag");
            }

            if (!data.ReleaseDate.HasValue)
                AddWarning($"{nameof(data.ReleaseDate)} is null");

            if (data.Rarity == Rarity.None || data.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(data.Rarity)} is {data.Rarity}");
        }
    }
}
