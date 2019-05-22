using Heroes.Models;

namespace HeroesData.FileWriter.Writers.PortraitData
{
    internal abstract class PortraitDataWriter<T, TU> : WriterBase<Portrait, T>
        where T : class
        where TU : class
    {
        protected PortraitDataWriter(FileOutputType fileOutputType)
            : base(nameof(PortraitData), fileOutputType)
        {
        }

        protected void AddLocalizedGameString(Portrait portrait)
        {
            GameStringWriter.AddPortraitName(portrait.Id, portrait.Name);
            GameStringWriter.AddPortraitSortName(portrait.Id, portrait.SortName);
        }
    }
}
