using Heroes.Models;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace HeroesData.FileWriter.Writers.HeroSkinData
{
    internal class HeroSkinDataJsonWriter : HeroSkinDataWriter<JProperty, JObject>
    {
        public HeroSkinDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(HeroSkin heroSkin)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(heroSkin);

            JObject heroSkinObject = new JObject();

            if (!string.IsNullOrEmpty(heroSkin.Name) && !FileOutputOptions.IsLocalizedText)
                heroSkinObject.Add("name", heroSkin.Name);

            heroSkinObject.Add("hyperlinkId", heroSkin.HyperlinkId);
            heroSkinObject.Add("attributeId", heroSkin.AttributeId);
            heroSkinObject.Add("rarity", heroSkin.Rarity.ToString());

            if (heroSkin.ReleaseDate.HasValue)
                heroSkinObject.Add("releaseDate", heroSkin.ReleaseDate.Value.ToString("yyyy-MM-dd"));

            if (!string.IsNullOrEmpty(heroSkin.SortName) && !FileOutputOptions.IsLocalizedText)
                heroSkinObject.Add("sortName", heroSkin.SortName);

            if (!string.IsNullOrEmpty(heroSkin.SearchText) && !FileOutputOptions.IsLocalizedText)
                heroSkinObject.Add("searchText", heroSkin.SearchText);

            if (!string.IsNullOrEmpty(heroSkin.InfoText?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                heroSkinObject.Add("infoText", GetTooltip(heroSkin.InfoText, FileOutputOptions.DescriptionType));

            if (heroSkin.Features.Any())
                heroSkinObject.Add(new JProperty("features", heroSkin.Features));

            return new JProperty(heroSkin.Id, heroSkinObject);
        }
    }
}
