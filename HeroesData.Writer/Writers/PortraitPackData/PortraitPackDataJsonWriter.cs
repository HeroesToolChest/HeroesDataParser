using Heroes.Models;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace HeroesData.FileWriter.Writers.PortraitPackData
{
    internal class PortraitPackDataJsonWriter : PortraitPackDataWriter<JProperty, JObject>
    {
        public PortraitPackDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(PortraitPack portrait)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(portrait);

            JObject portraitObject = new JObject();

            if (!string.IsNullOrEmpty(portrait.Name) && !FileOutputOptions.IsLocalizedText)
                portraitObject.Add("name", portrait.Name);

            portraitObject.Add("hyperlinkId", portrait.HyperlinkId);
            portraitObject.Add("rarity", portrait.Rarity.ToString());

            if (!string.IsNullOrEmpty(portrait.EventName))
                portraitObject.Add("event", portrait.EventName);

            if (!string.IsNullOrEmpty(portrait.SortName) && !FileOutputOptions.IsLocalizedText)
                portraitObject.Add("sortName", portrait.SortName);

            if (portrait.RewardPortraitIds != null && portrait.RewardPortraitIds.Any())
                portraitObject.Add(new JProperty("rewardPortraitIds", portrait.RewardPortraitIds));

            return new JProperty(portrait.Id, portraitObject);
        }
    }
}
