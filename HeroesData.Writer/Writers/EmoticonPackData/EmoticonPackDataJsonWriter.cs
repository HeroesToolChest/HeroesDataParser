using Heroes.Models;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace HeroesData.FileWriter.Writers.EmoticonPackData
{
    internal class EmoticonPackDataJsonWriter : EmoticonPackDataWriter<JProperty, JObject>
    {
        public EmoticonPackDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(EmoticonPack emoticonPack)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(emoticonPack);

            JObject emoticonObject = new JObject();

            if (!string.IsNullOrEmpty(emoticonPack.Name) && !FileOutputOptions.IsLocalizedText)
                emoticonObject.Add("name", emoticonPack.Name);

            if (!string.IsNullOrEmpty(emoticonPack.HyperlinkId))
                emoticonObject.Add("hyperlinkId", emoticonPack.HyperlinkId);

            if (!string.IsNullOrEmpty(emoticonPack.CollectionCategory))
                emoticonObject.Add("category", emoticonPack.CollectionCategory);

            if (!string.IsNullOrEmpty(emoticonPack.EventName))
                emoticonObject.Add("event", emoticonPack.EventName);

            if (emoticonPack.ReleaseDate.HasValue)
                emoticonObject.Add("releaseDate", emoticonPack.ReleaseDate.Value.ToString("yyyy-MM-dd"));

            if (!string.IsNullOrEmpty(emoticonPack.SortName) && !FileOutputOptions.IsLocalizedText)
                emoticonObject.Add("sortName", emoticonPack.SortName);

            if (!string.IsNullOrEmpty(emoticonPack.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                emoticonObject.Add("description", GetTooltip(emoticonPack.Description, FileOutputOptions.DescriptionType));

            if (emoticonPack.EmoticonIds != null && emoticonPack.EmoticonIds.Any())
                emoticonObject.Add(new JProperty("emoticons", emoticonPack.EmoticonIds));

            return new JProperty(emoticonPack.Id, emoticonObject);
        }
    }
}
