using Heroes.Models;

namespace HeroesData.FileWriter.Writers.VoiceLineData
{
    internal abstract class VoiceLineDataWriter<T, TU> : WriterBase<VoiceLine, T>
        where T : class
        where TU : class
    {
        public VoiceLineDataWriter(FileOutputType fileOutputType)
            : base(nameof(VoiceLineData), fileOutputType)
        {
        }

        protected void AddLocalizedGameString(VoiceLine voiceLine)
        {
            GameStringWriter.AddVoiceLineName(voiceLine.Id, voiceLine.Name);
            GameStringWriter.AddVoiceLineSortName(voiceLine.Id, voiceLine.SortName);
            GameStringWriter.AddVoiceLineDescription(voiceLine.Id, GetTooltip(voiceLine.Description, FileOutputOptions.DescriptionType));
        }
    }
}
