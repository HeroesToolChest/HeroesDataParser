using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataVoiceLine : DataExtractorBase<VoiceLine, VoiceLineParser>, IData
    {
        public DataVoiceLine(VoiceLineParser parser)
            : base(parser)
        {
        }

        public override string Name => "voicelines";

        protected override void Validation(VoiceLine voiceLine)
        {
            if (string.IsNullOrEmpty(voiceLine.Name))
                AddWarning($"{nameof(voiceLine.Name)} is empty");

            if (string.IsNullOrEmpty(voiceLine.Id))
                AddWarning($"{nameof(voiceLine.Id)} is empty");

            if (string.IsNullOrEmpty(voiceLine.AttributeId))
                AddWarning($"{nameof(voiceLine.AttributeId)} is empty");

            if (!voiceLine.ReleaseDate.HasValue)
                AddWarning($"{nameof(voiceLine.ReleaseDate)} is null");

            if (string.IsNullOrEmpty(voiceLine.ImageFileName))
                AddWarning($"{nameof(voiceLine.ImageFileName)} is empty");
        }
    }
}
