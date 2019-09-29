using Heroes.Models;
using HeroesData.Parser;
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

            if (string.IsNullOrEmpty(mount.Description?.RawDescription) && mount.Rarity == Rarity.None && mount.MountCategory == "Unique")
                AddWarning($"{nameof(mount.Description)} is empty");

            if (!mount.ReleaseDate.HasValue)
                AddWarning($"{nameof(mount.ReleaseDate)} is null");

            if (mount.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(mount.Rarity)} is unknown");
        }
    }
}
