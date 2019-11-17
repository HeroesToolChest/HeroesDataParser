using Heroes.Models;
using HeroesData.Parser;
using HeroesData.Parser.GameStrings;
using System;

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
            {
                throw new ArgumentNullException(nameof(mount));
            }

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

            if (mount.Rarity == Rarity.None && mount.MountCategory == "Unique")
            {
                if (string.IsNullOrEmpty(mount.Description?.RawDescription))
                    AddWarning($"{nameof(mount.Description)} is empty");
                else if (mount.Description.RawDescription == GameStringParser.FailedParsed)
                    AddWarning($"{nameof(mount.Description)} failed to parse correctly");
                else if (mount.Description.HasErrorTag)
                    AddWarning($"{nameof(mount.Description)} contains an error tag");
            }

            if (!mount.ReleaseDate.HasValue)
                AddWarning($"{nameof(mount.ReleaseDate)} is null");

            if (mount.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(mount.Rarity)} is unknown");
        }
    }
}
