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

            JObject sprayObject = new JObject();

            if (!string.IsNullOrEmpty(voiceLine.Name) && !FileOutputOptions.IsLocalizedText)
                sprayObject.Add("name", voiceLine.Name);

            sprayObject.Add("hyperlinkId", voiceLine.HyperlinkId);

            if (!string.IsNullOrEmpty(voiceLine.AttributeId))
                sprayObject.Add("attributeId", voiceLine.AttributeId);

            if (voiceLine.ReleaseDate.HasValue)
                sprayObject.Add("releaseDate", voiceLine.ReleaseDate.Value.ToString("yyyy-MM-dd"));

            if (!string.IsNullOrEmpty(voiceLine.SortName) && !FileOutputOptions.IsLocalizedText)
                sprayObject.Add("sortName", voiceLine.SortName);

            if (!string.IsNullOrEmpty(voiceLine.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                sprayObject.Add("description", GetTooltip(voiceLine.Description, FileOutputOptions.DescriptionType));

            return new JProperty(voiceLine.Id, sprayObject);
        }
    }
}
