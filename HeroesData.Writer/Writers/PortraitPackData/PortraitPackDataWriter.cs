using Heroes.Models;

namespace HeroesData.FileWriter.Writers.PortraitPackData
{
    internal abstract class PortraitPackDataWriter<T, TU> : WriterBase<PortraitPack, T>
        where T : class
        where TU : class
    {
        protected PortraitPackDataWriter(FileOutputType fileOutputType)
            : base(nameof(PortraitPackData), fileOutputType)
        {
        }

        protected void AddLocalizedGameString(PortraitPack portrait)
        {
            GameStringWriter.AddPortraitPackName(portrait.Id, portrait.Name);
            GameStringWriter.AddPortraitackSortName(portrait.Id, portrait.SortName);
        }
    }
}
