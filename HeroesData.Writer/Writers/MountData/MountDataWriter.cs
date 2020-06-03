using Heroes.Models;

namespace HeroesData.FileWriter.Writers.MountData
{
    internal abstract class MountDataWriter<T, TU> : WriterBase<Mount, T>
        where T : class
        where TU : class
    {
        protected MountDataWriter(FileOutputType fileOutputType)
            : base(nameof(MountData), fileOutputType)
        {
        }

        protected void AddLocalizedGameString(Mount mount)
        {
            GameStringWriter.AddMountName(mount.Id, mount.Name);
            GameStringWriter.AddMountSortName(mount.Id, mount.SortName);

            if (mount.InfoText != null)
                GameStringWriter.AddMountInfoText(mount.Id, GetTooltip(mount.InfoText, FileOutputOptions.DescriptionType));

            GameStringWriter.AddMountSearchText(mount.Id, mount.SearchText);
        }
    }
}
