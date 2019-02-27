using Heroes.Models;

namespace HeroesData.FileWriter.Writers.MountData
{
    internal abstract class MountDataWriter<T, TU> : WriterBase<Mount, T>
        where T : class
        where TU : class
    {
        public MountDataWriter(FileOutputType fileOutputType)
            : base(nameof(MountData), fileOutputType)
        {
        }

        protected void AddLocalizedGameString(Mount mount)
        {
            GameStringWriter.AddMountName(mount.ShortName, mount.Name);
            GameStringWriter.AddMountSortName(mount.ShortName, mount.SortName);
            GameStringWriter.AddMountInfo(mount.ShortName, GetTooltip(mount.Description, FileOutputOptions.DescriptionType));
            GameStringWriter.AddMountSearchText(mount.ShortName, mount.SearchText);
        }
    }
}
