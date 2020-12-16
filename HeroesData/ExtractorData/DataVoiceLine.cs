using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataVoiceLine : DataExtractorBase<VoiceLine?, VoiceLineParser>, IData
    {
        public DataVoiceLine(VoiceLineParser parser)
            : base(parser)
        {
        }

        public override string Name => "voicelines";

        protected override void Validation(VoiceLine? data)
        {
            if (data is null)
                return;

            if (string.IsNullOrEmpty(data.Name))
                AddWarning($"{nameof(data.Name)} is empty");

            if (string.IsNullOrEmpty(data.Id))
                AddWarning($"{nameof(data.Id)} is empty");

            if (string.IsNullOrEmpty(data.AttributeId))
                AddWarning($"{nameof(data.AttributeId)} is empty");

            if (!data.ReleaseDate.HasValue)
                AddWarning($"{nameof(data.ReleaseDate)} is null");

            if (string.IsNullOrEmpty(data.ImageFileName))
                AddWarning($"{nameof(data.ImageFileName)} is empty");

            if (data.Rarity == Rarity.None || data.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(data.Rarity)} is {data.Rarity}");
        }
    }
}
