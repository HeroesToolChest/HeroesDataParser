using Heroes.Models;
using Newtonsoft.Json.Linq;

namespace HeroesData.FileWriter.Writers.VoiceLineData
{
    internal class VoiceLineDataJsonWriter : VoiceLineDataWriter<JProperty, JObject>
    {
        public VoiceLineDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(VoiceLine voiceLine)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(voiceLine);

            JObject voiceLineObject = new JObject();

            if (!string.IsNullOrEmpty(voiceLine.Name) && !FileOutputOptions.IsLocalizedText)
                voiceLineObject.Add("name", voiceLine.Name);

            voiceLineObject.Add("hyperlinkId", voiceLine.HyperlinkId);

            if (!string.IsNullOrEmpty(voiceLine.AttributeId))
                voiceLineObject.Add("attributeId", voiceLine.AttributeId);

            voiceLineObject.Add("rarity", voiceLine.Rarity.ToString());

            if (voiceLine.ReleaseDate.HasValue)
                voiceLineObject.Add("releaseDate", voiceLine.ReleaseDate.Value.ToString("yyyy-MM-dd"));

            if (!string.IsNullOrEmpty(voiceLine.SortName) && !FileOutputOptions.IsLocalizedText)
                voiceLineObject.Add("sortName", voiceLine.SortName);

            if (!string.IsNullOrEmpty(voiceLine.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                voiceLineObject.Add("description", GetTooltip(voiceLine.Description, FileOutputOptions.DescriptionType));

            return new JProperty(voiceLine.Id, voiceLineObject);
        }
    }
}
