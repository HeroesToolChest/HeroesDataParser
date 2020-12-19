using Heroes.Models;
using Newtonsoft.Json.Linq;
namespace HeroesData.FileWriter.Writers.BoostData
{
    internal class BoostDataJsonWriter : BoostDataWriter<JProperty, JObject>
    {
        public BoostDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(Boost boost)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(boost);

            JObject boostObject = new JObject();

            if (!string.IsNullOrEmpty(boost.Name) && !FileOutputOptions.IsLocalizedText)
                boostObject.Add("name", boost.Name);

            boostObject.Add("hyperlinkId", boost.HyperlinkId);

            if (boost.ReleaseDate.HasValue)
                boostObject.Add("releaseDate", boost.ReleaseDate.Value.ToString("yyyy-MM-dd"));

            if (!string.IsNullOrEmpty(boost.SortName) && !FileOutputOptions.IsLocalizedText)
                boostObject.Add("sortName", boost.SortName);

            if (!string.IsNullOrEmpty(boost.EventName))
                boostObject.Add("event", boost.EventName);

            return new JProperty(boost.Id, boostObject);
        }
    }
}
