using Heroes.Models;
using Newtonsoft.Json.Linq;

namespace HeroesData.FileWriter.Writers.BannerData
{
    internal class BannerDataJsonWriter : BannerDataWriter<JProperty, JObject>
    {
        public BannerDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(Banner banner)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(banner);

            JObject bannerObject = new JObject();

            if (!string.IsNullOrEmpty(banner.Name) && !FileOutputOptions.IsLocalizedText)
                bannerObject.Add("name", banner.Name);

            if (!string.IsNullOrEmpty(banner.SortName) && !FileOutputOptions.IsLocalizedText)
                bannerObject.Add("sortName", banner.SortName);

            bannerObject.Add("hyperlinkId", banner.HyperlinkId);
            bannerObject.Add("attributeId", banner.AttributeId);
            bannerObject.Add("rarity", banner.Rarity.ToString());

            if (banner.ReleaseDate.HasValue)
                bannerObject.Add("releaseDate", banner.ReleaseDate.Value.ToString("yyyy-MM-dd"));

            bannerObject.Add("category", banner.CollectionCategory);

            if (!string.IsNullOrEmpty(banner.EventName))
                bannerObject.Add("event", banner.EventName);





            if (!string.IsNullOrEmpty(banner.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                bannerObject.Add("description", GetTooltip(banner.Description, FileOutputOptions.DescriptionType));

            return new JProperty(banner.Id, bannerObject);
        }
    }
}
