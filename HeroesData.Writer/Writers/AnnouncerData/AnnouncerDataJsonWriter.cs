using Heroes.Models;
using Newtonsoft.Json.Linq;
using System.IO;

namespace HeroesData.FileWriter.Writers.AnnouncerData
{
    internal class AnnouncerDataJsonWriter : AnnouncerDataWriter<JProperty, JObject>
    {
        public AnnouncerDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(Announcer announcer)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(announcer);

            JObject announcerObject = new JObject();

            if (!string.IsNullOrEmpty(announcer.Name) && !FileOutputOptions.IsLocalizedText)
                announcerObject.Add("name", announcer.Name);

            announcerObject.Add("hyperlinkId", announcer.HyperlinkId);
            announcerObject.Add("attributeId", announcer.AttributeId);
            announcerObject.Add("rarity", announcer.Rarity.ToString());
            announcerObject.Add("category", announcer.CollectionCategory);

            if (!string.IsNullOrEmpty(announcer.Gender))
                announcerObject.Add("gender", announcer.Gender);

            announcerObject.Add("hero", announcer.Hero);

            if (announcer.ReleaseDate.HasValue)
                announcerObject.Add("releaseDate", announcer.ReleaseDate.Value.ToString("yyyy-MM-dd"));

            if (!string.IsNullOrEmpty(announcer.SortName) && !FileOutputOptions.IsLocalizedText)
                announcerObject.Add("sortName", announcer.SortName);

            if (!string.IsNullOrEmpty(announcer.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                announcerObject.Add("description", GetTooltip(announcer.Description, FileOutputOptions.DescriptionType));

            announcerObject.Add("image", Path.ChangeExtension(announcer.ImageFileName, ImageExtension));

            return new JProperty(announcer.Id, announcerObject);
        }
    }
}
