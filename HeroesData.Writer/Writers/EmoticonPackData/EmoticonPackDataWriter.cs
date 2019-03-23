using Heroes.Models;

namespace HeroesData.FileWriter.Writers.EmoticonPackData
{
    internal abstract class EmoticonPackDataWriter<T, TU> : WriterBase<EmoticonPack, T>
        where T : class
        where TU : class
    {
        public EmoticonPackDataWriter(FileOutputType fileOutputType)
            : base(nameof(EmoticonPackData), fileOutputType)
        {
        }

        protected void AddLocalizedGameString(EmoticonPack emoticonPack)
        {
            GameStringWriter.AddEmoticonPackName(emoticonPack.Id, emoticonPack.Name);
            GameStringWriter.AddEmoticonPackDescription(emoticonPack.Id, GetTooltip(emoticonPack.Description, FileOutputOptions.DescriptionType));
            GameStringWriter.AddEmoticonPackSortName(emoticonPack.Id, emoticonPack.SortName);
        }
    }
}
