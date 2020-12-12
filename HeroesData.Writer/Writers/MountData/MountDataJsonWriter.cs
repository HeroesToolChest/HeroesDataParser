using Heroes.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace HeroesData.FileWriter.Writers.MountData
{
    internal class MountDataJsonWriter : MountDataWriter<JProperty, JObject>
    {
        public MountDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(Mount mount)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(mount);

            JObject mountObject = new JObject();

            if (!string.IsNullOrEmpty(mount.Name) && !FileOutputOptions.IsLocalizedText)
                mountObject.Add("name", mount.Name);

            mountObject.Add("hyperlinkId", mount.HyperlinkId);
            mountObject.Add("attributeId", mount.AttributeId);
            mountObject.Add("rarity", mount.Rarity.ToString());
            mountObject.Add("type", mount.MountCategory);
            mountObject.Add("category", mount.CollectionCategory);

            if (!string.IsNullOrEmpty(mount.EventName))
                mountObject.Add("event", mount.EventName);

            if (mount.ReleaseDate.HasValue)
                mountObject.Add("releaseDate", mount.ReleaseDate.Value.ToString("yyyy-MM-dd"));

            if (!string.IsNullOrEmpty(mount.SortName) && !FileOutputOptions.IsLocalizedText)
                mountObject.Add("sortName", mount.SortName);

            if (!string.IsNullOrEmpty(mount.SearchText) && !FileOutputOptions.IsLocalizedText)
                mountObject.Add("searchText", mount.SearchText);

            if (!string.IsNullOrEmpty(mount.InfoText?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                mountObject.Add("infoText", GetTooltip(mount.InfoText, FileOutputOptions.DescriptionType));

            mountObject.Add("franchise", mount.Franchise.ToString());

            if (mount.VariationMountIds.Count > 0)
                mountObject.Add(new JProperty("variationMountIds", mount.VariationMountIds.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)));

            return new JProperty(mount.Id, mountObject);
        }
    }
}
