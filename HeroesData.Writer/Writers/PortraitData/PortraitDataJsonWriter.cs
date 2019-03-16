using Heroes.Models;
using Newtonsoft.Json.Linq;

namespace HeroesData.FileWriter.Writers.PortraitData
{
    internal class PortraitDataJsonWriter : PortraitDataWriter<JProperty, JObject>
    {
        public PortraitDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(Portrait portrait)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(portrait);

            JObject portraitObject = new JObject();

            if (!string.IsNullOrEmpty(portrait.Name) && !FileOutputOptions.IsLocalizedText)
                portraitObject.Add("name", portrait.Name);

            portraitObject.Add("hyperlinkId", portrait.HyperlinkId);

            if (!string.IsNullOrEmpty(portrait.EventName))
                portraitObject.Add("event", portrait.EventName);

            if (!string.IsNullOrEmpty(portrait.SortName) && !FileOutputOptions.IsLocalizedText)
                portraitObject.Add("sortName", portrait.SortName);

            return new JProperty(portrait.Id, portraitObject);
        }
    }
}
