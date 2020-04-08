using Heroes.Models;
using Newtonsoft.Json.Linq;
using System.IO;

namespace HeroesData.FileWriter.Writers.MatchAwardData
{
    internal class MatchAwardDataJsonWriter : MatchAwardDataWriter<JProperty, JObject>
    {
        public MatchAwardDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(MatchAward matchAward)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(matchAward);

            JObject matchAwardObject = new JObject();

            if (!string.IsNullOrEmpty(matchAward.Name) && !FileOutputOptions.IsLocalizedText)
                matchAwardObject.Add("name", matchAward.Name);

            matchAwardObject.Add("gameLink", matchAward.HyperlinkId);
            matchAwardObject.Add("tag", matchAward.Tag);
            matchAwardObject.Add("mvpScreenIcon", Path.ChangeExtension(matchAward.MVPScreenImageFileName?.ToLowerInvariant(), StaticImageExtension));
            matchAwardObject.Add("scoreScreenIcon", Path.ChangeExtension(matchAward.ScoreScreenImageFileName?.ToLowerInvariant(), StaticImageExtension));

            if (!FileOutputOptions.IsLocalizedText && matchAward.Description != null)
                matchAwardObject.Add("description", GetTooltip(matchAward.Description, FileOutputOptions.DescriptionType));

            return new JProperty(matchAward.Id, matchAwardObject);
        }
    }
}
