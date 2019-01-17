using Heroes.Models;
using Newtonsoft.Json.Linq;
using System.IO;

namespace HeroesData.FileWriter.Writer.MatchAwardData
{
    internal class MatchAwardDataJsonWriter : MatchAwardDataWriter<JProperty, JObject>
    {
        public MatchAwardDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(MatchAward matchAward)
        {
            if (IsLocalizedText)
                AddLocalizedGameString(matchAward);

            JObject matchAwardObject = new JObject();

            if (!string.IsNullOrEmpty(matchAward.Name) && !IsLocalizedText)
                matchAwardObject.Add("name", matchAward.Name);

            matchAwardObject.Add("tag", matchAward.Tag);
            matchAwardObject.Add("mvpScreenIcon", Path.ChangeExtension(matchAward.MVPScreenImageFileName, FileSettings.ImageExtension));
            matchAwardObject.Add("scoreScreenIcon", Path.ChangeExtension(matchAward.ScoreScreenImageFileName, FileSettings.ImageExtension));

            if (!IsLocalizedText)
                matchAwardObject.Add("description", GetTooltip(matchAward.Description, FileSettings.DescriptionType));

            return new JProperty(matchAward.ShortName, matchAwardObject);
        }
    }
}
