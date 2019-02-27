using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataMount : DataExtractorBase<Mount, MountParser>, IData
    {
        public DataMount(MountParser parser)
            : base(parser)
        {
        }

        public override string Name => "mounts";

        protected override void Validation(Mount mount)
        {
            if (string.IsNullOrEmpty(mount.Name))
                AddWarning($"{nameof(mount.Name)} is null or empty");

            if (string.IsNullOrEmpty(mount.MountId))
                AddWarning($"{nameof(mount.MountId)} is null or empty");

            if (string.IsNullOrEmpty(mount.ShortName))
                AddWarning($"{nameof(mount.ShortName)} is null or empty");

            if (string.IsNullOrEmpty(mount.SortName))
                AddWarning($"{nameof(mount.SortName)} is null or empty");

            if (string.IsNullOrEmpty(mount.AttributeId))
                AddWarning($"{nameof(mount.AttributeId)} is null or empty");

            if (string.IsNullOrEmpty(mount.Description?.RawDescription))
                AddWarning($"{nameof(mount.Description)} is null or empty");

            if (string.IsNullOrEmpty(mount.SearchText))
                AddWarning($"{nameof(mount.SearchText)} is null or empty");

            if (!mount.ReleaseDate.HasValue)
                AddWarning($"{nameof(mount.ReleaseDate)} is null");
        }
    }
}
