using Heroes.Models;

namespace HeroesData.FileWriter.Writers.BoostData
{
    internal abstract class BoostDataWriter<T, TU> : WriterBase<Boost, T>
        where T : class
        where TU : class
    {
        protected BoostDataWriter(FileOutputType fileOutputType)
            : base(nameof(BoostData), fileOutputType)
        {
        }

        protected void AddLocalizedGameString(Boost boost)
        {
            GameStringWriter.AddBoostName(boost.Id, boost.Name);
            GameStringWriter.AddBoostSortName(boost.Id, boost.SortName);
        }
    }
}
