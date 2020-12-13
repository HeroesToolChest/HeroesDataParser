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

        protected override void Validation(Mount? mount)
        {
            if (mount is null)
                return;

            if (string.IsNullOrEmpty(mount.Name))
                AddWarning($"{nameof(mount.Name)} is empty");

            if (string.IsNullOrEmpty(mount.Id))
                AddWarning($"{nameof(mount.Id)} is empty");

            if (string.IsNullOrEmpty(mount.HyperlinkId))
                AddWarning($"{nameof(mount.HyperlinkId)} is empty");

            if (string.IsNullOrEmpty(mount.AttributeId))
                AddWarning($"{nameof(mount.AttributeId)} is empty");

            if (string.IsNullOrEmpty(mount.MountCategory))
                AddWarning($"{nameof(mount.MountCategory)} is empty");

            if (string.IsNullOrEmpty(mount.CollectionCategory))
                AddWarning($"{nameof(mount.CollectionCategory)} is empty");

            if (mount.MountCategory == "Unique")
            {
                if (string.IsNullOrEmpty(mount.InfoText?.RawDescription))
                    AddWarning($"{nameof(mount.InfoText)} is empty");
                else if (mount.InfoText.RawDescription == GameStringParser.FailedParsed)
                    AddWarning($"{nameof(mount.InfoText)} failed to parse correctly");
                else if (mount.InfoText.HasErrorTag)
                    AddWarning($"{nameof(mount.InfoText)} contains an error tag");
            }

            if (!mount.ReleaseDate.HasValue)
                AddWarning($"{nameof(mount.ReleaseDate)} is null");

            if (mount.Rarity == Rarity.None || mount.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(mount.Rarity)} is {mount.Rarity}");

            if (mount.Franchise == Franchise.Unknown)
                AddWarning($"{nameof(mount.Franchise)} is {mount.Franchise}");
        }
    }
}
