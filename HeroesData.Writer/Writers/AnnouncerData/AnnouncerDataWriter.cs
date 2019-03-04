using Heroes.Models;

namespace HeroesData.FileWriter.Writers.AnnouncerData
{
    internal abstract class AnnouncerDataWriter<T, TU> : WriterBase<Announcer, T>
        where T : class
        where TU : class
    {
        public AnnouncerDataWriter(FileOutputType fileOutputType)
            : base(nameof(AnnouncerData), fileOutputType)
        {
        }

        protected void AddLocalizedGameString(Announcer announcer)
        {
            GameStringWriter.AddAnnouncerName(announcer.Id, announcer.Name);
            GameStringWriter.AddAnnouncerSortName(announcer.Id, announcer.SortName);
            GameStringWriter.AddAnnouncerDescription(announcer.Id, GetTooltip(announcer.Description, FileOutputOptions.DescriptionType));
        }
    }
}
